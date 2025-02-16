﻿using poetrain.Data;
using poetrain.Markov;
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
        public string IndexSrcName { get; }
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

    public interface IPhoneticDictionary : IEnumerable<ITranscription>
    {
        public IPhonologyProvider Provider { get; }
        public IPALanguage Language { get; }
        public float StressScoreAggregationWeight { get; }
        public bool ContainsWord(string word);
        public ITranscription? TryGetTranscription(string word);
    }

    public interface IReversePhoneticDictionary
    {
        public IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation);
        public IEnumerable<IPronnunciation> FindRhymes(VowelString vowelString);
        public IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation, IPredictionTable markov);
    }

    public interface ITranscription : IEnumerable<IPronnunciation>
    {
        public IPhoneticDictionary Dictionary { get; }
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

        public static ITranscription Concat(ITranscription a, ITranscription b, IPhoneticDictionary? dict = null)
        {
            var provider = a.Provider;
            if (provider != b.Provider)
                throw new ArgumentException("The transcriptions are not from the same providers");
            if (dict == null)
                dict = a.Dictionary;
            var aPronnunciations = a
                .Select(p => p.Data);
            var bPronnunciations = b
                .Select(p => p.Data);
            var concatPronnunciations = aPronnunciations
                .SelectMany(x => bPronnunciations
                .Select(y => PronnunciationData.Concat(x, y)));
            return new Transcription(dict, $"{a.Word} {b.Word}", concatPronnunciations);
        }
    }

    public interface IPronnunciation : IEnumerable<ISemiSyllable>
    {
        public IPhonologyProvider Provider { get; }
        public ITranscription Transcription { get; }
        public int PhonymCount { get; }
        public int SyllableCount { get; }
        public ISemiSyllable this[int phonymIndex] { get; }
        public PronnunciationData Data { get; }
        public VowelString ToVowelString();

        public static IPronnunciation Concat(IPronnunciation a, IPronnunciation b)
        {
            var provider = a.Provider;
            if (provider != b.Provider)
                throw new ArgumentException("The transcriptions are not from the same providers");
            var aTranscrip = a.Transcription;
            var bTranscrip = b.Transcription;
            var concatTranscrip = new Transcription(aTranscrip.Dictionary, $"{aTranscrip.Word} {bTranscrip.Word}", YieldConcatPronnunc(a, b));
            return concatTranscrip[0];
        }

        private static IEnumerable<PronnunciationData> YieldConcatPronnunc(IPronnunciation a, IPronnunciation b)
        {
            yield return PronnunciationData.Concat(a.Data, b.Data);
        }
    }

    public interface IPronnunciationData
    {
        public int SyllableCount { get; }
        public ISyllableData[] Body { get; }
        public ISemiSyllable[] Cap { get; }
    }

    public interface ISyllableData
    {
        public ISemiSyllable[] BeginConsonants { get; }
        public ISemiSyllable Vowel { get; }
        public ISemiSyllable? EndConsonant { get; }
        public SyllableStress Stress { get; }

        public static float ScoreRhyme<TSyllableData>(IPhonologyProvider provider, TSyllableData a, TSyllableData b) where TSyllableData : ISyllableData
        {

        }

        private static float ScoreVowels<TSyllableData>(TSyllableData a, TSyllableData b) where TSyllableData : ISyllableData
        {
            return a.Vowel.ScoreRhyme(b.Vowel);
        }

        private static float ScoreBeginConsonants<TSyllableData>(TSyllableData a, TSyllableData b) where TSyllableData : ISyllableData
        {
            var larger = a.BeginConsonants.Length > b.BeginConsonants.Length ? a.BeginConsonants : b.BeginConsonants;
            var smaller = a.BeginConsonants.Length > b.BeginConsonants.Length ? b.BeginConsonants : a.BeginConsonants;
            var sum = 0f;
            foreach (var phonym in larger)
            {
                sum += smaller
                    .Select(phonym.ScoreRhyme)
                    .Max();
            }
            return sum / larger.Length;
        }

        public static float ScoreEndConsonant<TSyllableData>(TSyllableData a, TSyllableData b) where TSyllableData : ISyllableData
        {
            var basicScore = (a.EndConsonant == null || b.EndConsonant == null) ? 0.5f :
                a.EndConsonant.ScoreRhyme(b.EndConsonant);
            var aConsonantShiftScore = a.EndConsonant == null ? 0f :
                b.BeginConsonants
                .Select(a.EndConsonant.ScoreRhyme)
                .Max();
            var bConsonantShiftScore = b.EndConsonant == null ? 0f :
                a.BeginConsonants
                .Select(b.EndConsonant.ScoreRhyme)
                .Max();
            return Math.Max(basicScore, 0.5f * Math.Max(aConsonantShiftScore, bConsonantShiftScore));
        }
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
