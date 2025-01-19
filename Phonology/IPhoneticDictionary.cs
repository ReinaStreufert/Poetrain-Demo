using poetrain.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public static class PhoneticsProviderExtensions
    {
        public static IPhoneticDictionary? TryFromCultureInfo(this IPhonologyProvider phoneticsProvider, CultureInfo? cultureInfo = null)
        {
            if (cultureInfo == null)
                cultureInfo = CultureInfo.CurrentCulture;
            return phoneticsProvider.TryLoadLocale(cultureInfo.Name);
        }
    }

    public interface ISlantGrid
    {
        public float RhymeScoreFromDistance(float dist);
    }

    public interface IIPAConfig
    {
        public ScoreAggregationWeights ScoreAggregation { get; }
        public MarkupChars Markup { get; }
    }

    public interface IIPAPhonologyData : IIPAConfig
    {
        public IEnumerable<IPALanguage> Languages { get; }
        public ILocalizationPhonology? TryGetLocale(string language);
    }

    public interface ILocalizationPhonology
    {
        public string DictionarySrcName { get; }
        public IPALanguage Language { get; }
        public ISemiSyllable? TrySemiSyllableFromIPA(string ipa);
        public IIPAConfig Config { get; }
    }

    public interface IPhonologyProvider
    {
        public ScoreAggregationWeights ScoreAggregation { get; }
        public IEnumerable<IPALanguage> Languages { get; }
        public IPhoneticDictionary? TryLoadLocale(string languageCode);
        public IPhoneticDictionary LoadLocale(string languageCode);
    }

    public interface IPhoneticDictionary
    {
        public IPALanguage Language { get; }
        public float StressScoreAggregationWeight { get; }
        public IPhonologyProvider Provider { get; }
        public ITranscription? TryGetTranscription(string word);
    }

    public interface ITranscription : IEnumerable<IPronnunciation>
    {
        public IPhonologyProvider Provider { get; }
        public string Word { get; }
        public int PronnunciationCount { get; }
        public IPronnunciation this[int index] { get; }

        public static ITranscription Concat(IEnumerable<ITranscription> transcriptions)
        {
            ITranscription? current = null;
            foreach (var transcript in transcriptions)
            {
                if (current == null)
                    current = transcript;
                else
                    current = Concat(current, transcript);
            }
            return current ?? throw new ArgumentException($"{nameof(transcriptions)} is an empty enumeration");
        }

        public static ITranscription Concat(ITranscription a, ITranscription b)
        {
            var provider = a.Provider;
            if (provider != b.Provider)
                throw new ArgumentException("The transcriptions are not from the same providers");
            var pronnunciationTotal = a.PronnunciationCount * b.PronnunciationCount;
            ISyllable[][] pronnunciationsArr = new ISyllable[pronnunciationTotal][];
            for (int aIndex = 0; aIndex < a.PronnunciationCount; aIndex++)
            {
                for (int bIndex = 0; bIndex < b.PronnunciationCount; bIndex++)
                {
                    var i = aIndex * b.PronnunciationCount + bIndex;
                    var aPronnunciation = a[aIndex].ToSyllableArray();
                    var bPronnunciation = b[bIndex].ToSyllableArray();
                    pronnunciationsArr[i] = ConcatSyllables(aPronnunciation, bPronnunciation);
                }
            }
            return new Transcription(provider, $"{a.Word} {b.Word}", pronnunciationsArr);
        }

        private static ISyllable[] ConcatSyllables(ISyllable[] seqA, ISyllable[] seqB)
        {
            var aLastSyll = seqA[seqA.Length - 1];
            var bFirstSyll = seqB[0];
            var linkfixConsonants = Concat(
                aLastSyll.PostfixConsonants,
                bFirstSyll.PrefixConsonants);
            seqA[seqA.Length - 1] = new Syllable(aLastSyll.Provider, aLastSyll.Stress, aLastSyll.PrefixConsonants, linkfixConsonants, aLastSyll.VowelBridge);
            seqB[0] = new Syllable(bFirstSyll.Provider, bFirstSyll.Stress, linkfixConsonants, bFirstSyll.PostfixConsonants, bFirstSyll.VowelBridge);
            return Concat(seqA, seqB);
        }

        private static T[] Concat<T>(T[] a, T[] b)
        {
            var result = new T[a.Length + b.Length];
            for (int i = 0; i < a.Length; i++)
                result[i] = a[i];
            for (int i = 0; i < b.Length; i++)
                result[a.Length + i] = b[i];
            return result;
        }
    }

    public interface IPronnunciation : IEnumerable<ISyllable>
    {
        public IPhonologyProvider Provider { get; }
        public ITranscription Transcription { get; }
        public int SyllableCount { get; }
        public ISyllable this[int index] { get; }
    }

    public interface ISyllable
    {
        public IPhonologyProvider Provider { get; }
        public ISemiSyllable[] PrefixConsonants { get; }
        public ISemiSyllable[] PostfixConsonants { get; }
        public ISemiSyllable VowelBridge { get; }
        public SyllableStress Stress { get; }
    }

    public interface ISemiSyllable
    {
        public SemiSyllableType Type { get; }
        public string Name { get; }
        public string IPAString { get; }
        public float ScoreRhyme(ISemiSyllable semiSyllable);
        protected float ScoreRhyme(SemiSyllableType type, SlantCoords slantCoords);

        protected static float ScoreRhyme(ISemiSyllable semiSyllable, SemiSyllableType rhymeType, SlantCoords slantCoords)
        {
            return semiSyllable.ScoreRhyme(rhymeType, slantCoords);
        }
    }

    public interface IMonoPhonym : ISemiSyllable
    {
        public SlantCoords SlantCoords { get; }
    }

    public interface ITranscriptionBuilder
    {
        public void AddPronnunciationsFromString(string ipaString, ILocalizationPhonology alphabet);
        public void Clear();
        public ITranscription ToTranscription(string word);
    }

    public interface ITranscriptionBuildContext
    {
        public void Consonant(ISemiSyllable consonant);
        public void Vowel(ISemiSyllable vowel);
        public void Stress(SyllableStress stressType);
        public void EndPronnunciation();
    }

    public interface ITranscriptionToken
    {
        public void WriteTo(ITranscriptionBuildContext transcriptionBuilder);
    }

    public enum SyllableStress
    {
        Unstressed = 0,
        Secondary = 1,
        Primary = 2,
    }
    
    public enum SemiSyllableType
    {
        Consonant,
        Vowel
    }
}
