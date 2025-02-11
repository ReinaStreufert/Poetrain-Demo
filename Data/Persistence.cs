﻿using Microsoft.VisualBasic.Devices;
using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace poetrain.Data
{
    public static class Persistence
    {
        private const string AppDataSubdirName = "poetrain";
        private const string StatsFileName = "stats.xml";
        private const string StatsDefaultSourceName = "statsDefault.xml";

        public static string DataDirPath
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataSubdirName);
                Directory.CreateDirectory(path); // does nothing if already exists
                return path;
            }
        }

        public static int HighScore
        {
            get
            {
                Load();
                return _HighScore;
            }
        }

        private static Dictionary<string, HashSet<string>>? _PastRhymes;
        private static int _HighScore;
        private static bool _StatsLoaded = false;

        public static void Save()
        {
            Load();
            var xmlDocument = new XmlDocument();
            var statsXmlNode = xmlDocument.CreateElement("poetrainStats");
            statsXmlNode.SetAttribute("highscore", _HighScore.ToString());
            var keywordNodes = _PastRhymes!
                .Select(p => ToKeywordNode(p, xmlDocument));
            foreach (var n in keywordNodes)
                statsXmlNode.AppendChild(n);
            xmlDocument.AppendChild(statsXmlNode);

            var statsXmlPath = Path.Combine(DataDirPath, StatsFileName);
            xmlDocument.Save(statsXmlPath);
        }

        public static IEnumerable<string>? GetPastRhymes(ITranscription challenge)
        {
            Load();
            var vowelStrings = challenge
                .Select(p => p.ToVowelString().ToString());
            var pastRhymeLists = vowelStrings
                .Select(text => _PastRhymes!.TryGetValue(text, out var result) ? result : null);
            if (!pastRhymeLists.Where(l => l != null).Any())
                return null;
            return pastRhymeLists
                .Where(l => l != null)
                .SelectMany(l => l!)
                .Distinct();
        }

        public static void RecordScore(int score)
        {
            Load();
            _HighScore = Math.Max(score, _HighScore);
        }

        public static void RecordRhyme(IPronnunciation challenge, string rhyme)
        {
            Load();
            var vowelString = challenge.ToVowelString().ToString();
            HashSet<string> pastRhymeList;
            if (!_PastRhymes!.TryGetValue(vowelString, out pastRhymeList!))
            {
                pastRhymeList = new HashSet<string>() { rhyme };
                _PastRhymes.Add(vowelString, pastRhymeList);
            }
            else
                pastRhymeList.Add(rhyme);
        }

        private static void Load()
        {
            if (_StatsLoaded)
                return;
            var statsXmlPath = Path.Combine(DataDirPath, StatsFileName);
            XmlDocument xmlDocument;
            if (File.Exists(statsXmlPath))
            {
                xmlDocument = new XmlDocument();
                xmlDocument.Load(statsXmlPath);
            } else
            {
                xmlDocument = EmbeddedSource.GetEmbeddedXml(StatsDefaultSourceName);
                xmlDocument.Save(statsXmlPath);
            }
            var statsNode = xmlDocument.GetElementsByTagName("poetrainStats")
                .OfType<XmlElement>()
                .First();
            _HighScore = int.Parse(statsNode.Attributes["highscore"]!.Value!);
            var pastRhymePairs = xmlDocument.GetElementsByTagName("keyRhyme")
                .OfType<XmlElement>()
                .Select(ParseKeyRhymeNode);
            _PastRhymes = new Dictionary<string, HashSet<string>>(pastRhymePairs);
            _StatsLoaded = true;
        }

        private static KeyValuePair<string, HashSet<string>> ParseKeyRhymeNode(XmlElement node)
        {
            var keyword = node.Attributes["syllableVowels"]!.Value!;
            var pastRhymes = node.GetElementsByTagName("pastRhyme")
                .OfType<XmlElement>()
                .Select(n => n.Attributes["word"]!.Value!)
                .ToHashSet();
            return new KeyValuePair<string, HashSet<string>>(keyword, pastRhymes);
        }

        private static XmlElement ToKeywordNode(KeyValuePair<string, HashSet<string>> pair, XmlDocument xmlDocument)
        {
            var node = xmlDocument.CreateElement("keyRhyme");
            node.SetAttribute("syllableVowels", pair.Key);
            foreach (var pastRhyme in pair.Value)
            {
                var pastRhymeNode = xmlDocument.CreateElement("pastRhyme");
                pastRhymeNode.SetAttribute("word", pastRhyme);
                node.AppendChild(pastRhymeNode);
            }
            return node;
        }
    }
}
