using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace poetrain.Data
{
    public static class IPAData
    {
        private const string _PhonologyDataSource = "ipaConfig.xml";

        public static IPAPhonologyData Phonology { get; } = ParsePhonologyData();

        public static IPAPhonologyData ParsePhonologyData(string sourceName = _PhonologyDataSource)
        {
            var src = EmbeddedSource.GetEmbeddedXml(sourceName);
            return ParsePhonologyData(src);
        }

        public static IPAPhonologyData ParsePhonologyData(XmlDocument phonologyDataXml)
        {
            var configNode = phonologyDataXml
                .GetElementsByTagName("ipaConfig")
                .OfType<XmlElement>()
                .First();
            var scoreAggrWeights = ParseScoreAggrWeights(configNode["scoring"]!);
            var markupChars = ParseMarkupChars(configNode["notation"]!);
            var config = new IPAConfig(scoreAggrWeights, markupChars);
            var localizations = configNode.GetElementsByTagName("localization")
                .OfType<XmlElement>()
                .Select(n => ParseLocalization(n, config))
                .Select(a => new KeyValuePair<string, ILocalizationPhonology>(a.Language.Code, a));
            return new IPAPhonologyData(markupChars, scoreAggrWeights, localizations);
        }

        public static IEnumerable<KeyValuePair<string, string>> ParseRawTranscriptionData(XmlDocument dictXml, string languageCode)
        {
            var dictNode = dictXml.GetElementsByTagName("IPADict")
                .OfType<XmlElement>()
                .First();
            var wordList = dictNode.GetElementsByTagName("WordList")
                .OfType<XmlElement>()
                .Where(n => n.Attributes["LangName"]!.Value! == languageCode)
                .First();
            return wordList.GetElementsByTagName("IpaEntry")
                .OfType<XmlElement>()
                .Select(ParseIPAEntry);
        }

        private static KeyValuePair<string, string> ParseIPAEntry(XmlElement ipaEntryNode)
        {
            var itemNode = ipaEntryNode.GetElementsByTagName("Item")
                .OfType<XmlElement>()
                .First();
            var ipaNode = ipaEntryNode.GetElementsByTagName("Ipa")
                .OfType<XmlElement>()
                .First();
            return new KeyValuePair<string, string>(itemNode.InnerText, ipaNode.InnerText);
        }

        private static ScoreAggregationWeights ParseScoreAggrWeights(XmlElement scoringNode)
        {
            var stressWeight = float.Parse(scoringNode["stress"]!.Attributes["importance"]!.Value);
            var consonantWeight = float.Parse(scoringNode["consonant"]!.Attributes["importance"]!.Value);
            var vowelWeight = float.Parse(scoringNode["vowel"]!.Attributes["importance"]!.Value);
            var sum = stressWeight + consonantWeight + vowelWeight;
            return new ScoreAggregationWeights(stressWeight / sum, consonantWeight / sum, vowelWeight / sum);
        }

        private static MarkupChars ParseMarkupChars(XmlElement notationNode)
        {
            var transcriptionQuote = notationNode["transcriptionQuote"]!.InnerText[0];
            var pronnunciationBreak = notationNode["pronnunciationBreak"]!.InnerText[0];
            var primaryStress = notationNode["primaryStress"]!.InnerText[0];
            var secondaryStress = notationNode["secondaryStress"]!.InnerText[0];
            return new MarkupChars(transcriptionQuote, pronnunciationBreak, primaryStress, secondaryStress);
        }

        private static LocalizationPhonology ParseLocalization(XmlElement localizationNode, IPAConfig config)
        {
            var consonantGrid = new SlantGrid();
            var monopthongGrid = new SlantGrid();
            var scoreAggrWeights = config.ScoreAggregation;
            var consonants = localizationNode
                .GetElementsByTagName("consonant")
                .OfType<XmlElement>()
                .Select(n => ParseMonoPhonym(n, SemiSyllableType.Consonant, consonantGrid))
                .ToArray();
            consonantGrid.UpdateMaxDistance(consonants);
            var monopthongs = localizationNode
                .GetElementsByTagName("monopthong")
                .OfType<XmlElement>()
                .Select(n => ParseMonoPhonym(n, SemiSyllableType.Vowel, monopthongGrid))
                .ToImmutableDictionary(n => n.Name);
            monopthongGrid.UpdateMaxDistance(monopthongs.Values);
            var dipthongs = localizationNode
                .GetElementsByTagName("dipthong")
                .OfType<XmlElement>()
                .Select(n => ParseDipthong(n, monopthongs));
            var attributes = localizationNode.Attributes;
            var langCode = attributes["code"]!.Value!;
            var langName = attributes["name"]!.Value!;
            var dictSrcName = attributes["dictSrcName"]!.Value!;
            var indexSrcName = attributes["indexSrcName"]!.Value!;
            var allPhonyms = consonants
                .Concat(monopthongs.Values)
                .Concat(dipthongs);
            var phonymPairs = allPhonyms.Select(p => new KeyValuePair<string, ISemiSyllable>(p.IPAString, p));
            return new LocalizationPhonology(new IPALanguage(langCode, langName), dictSrcName, indexSrcName, phonymPairs, config);
        }

        private static IMonoPhonym ParseMonoPhonym(XmlElement phonymNode, SemiSyllableType type, SlantGrid grid)
        {
            var attributes = phonymNode.Attributes;
            var name = attributes["name"]!.Value!;
            var ipa = attributes["ipa"]!.Value!;
            var slantCoords = SlantCoords.Parse(attributes["slantCoords"]!.Value!);
            return new MonoPhonym(name, type, ipa, slantCoords, grid);
        }

        private static ISemiSyllable ParseDipthong(XmlElement dipthongNode, ImmutableDictionary<string, IMonoPhonym> monoPhonyms)
        {
            var attributes = dipthongNode.Attributes;
            var name = attributes["name"]!.Value!;
            var ipa = attributes["ipa"]!.Value!;
            var start = monoPhonyms[attributes["start"]!.Value!];
            var end = monoPhonyms[attributes["end"]!.Value!];
            return new Dipthong(name, ipa, start, end);
        }
    }
}
