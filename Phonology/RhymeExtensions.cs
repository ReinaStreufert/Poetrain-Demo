using poetrain.Data;
using poetrain.Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public static class RhymeExtensions
    {
        private const int _MaxPredictions = 1000;

        public static RhymeScore ScoreRhyme(this ITranscription a, ITranscription b)
        {
            return a
                .SelectMany(x => b
                .Select(y => x
                .ScoreRhyme(y)))
                .MaxBy(s => s.Value) ?? throw new InvalidOperationException();
        }

        public static RhymeScore ScoreRhyme(this ITranscription a, IPronnunciation b)
        {
            return a
                .Select(p => p.ScoreRhyme(b))
                .MaxBy(s => s.Value) ?? throw new InvalidOperationException();
        }

        public static RhymeScore ScoreRhyme(this IPronnunciation a, IPronnunciation b)
        {
            var provider = a.Provider;
            if (provider != b.Provider)
                throw new InvalidOperationException("Pronnunciations are not comparable");
            var scoreAggr = provider.ScoreAggregation;
            var scoreVal = a.Data.ScoreRhyme(b.Data, scoreAggr);
            return new RhymeScore(scoreVal, a, b);
        }

        public static float ScoreRhyme(this PronnunciationData a, PronnunciationData b, ScoreAggregationWeights scoreAggr)
        {
            var vowelScore = a.ScoreVowels(b);
            var consonantScore = a.ScoreConsonants(b);
            var stressScore = a.ScoreStresses(b); // why does just this one need explicit generic?
            return scoreAggr.AggregateScores(stressScore, vowelScore, consonantScore);
        }

        public static float ScoreConsonants(this PronnunciationData a, PronnunciationData b)
        {
            var aSyllCount = a.SyllableCount;
            var bSyllCount = b.SyllableCount;
            if (aSyllCount == 0 || bSyllCount == 0)
                return 0f;
            var larger = aSyllCount > bSyllCount ? a : b;
            var alt = aSyllCount > bSyllCount ? b : a;
            var rangeCount = larger.SyllableCount + 1;
            var altOffset = larger.SyllableCount - alt.SyllableCount;
            var sum = 0f;
            for (int i = altOffset; i < rangeCount; i++)
            {
                var range = larger.GetConsonantRange(i);
                var closest = ScoreRhyme(range, alt.GetConsonantRange(i - altOffset));
                var past = ScoreRhyme(range, alt.GetConsonantRange(i - altOffset - 1));
                var future = ScoreRhyme(range, alt.GetConsonantRange(i - altOffset + 1));
                sum += Math.Max(closest, 0.5f * Math.Max(past, future));
            }
            return rangeCount > 0 ? sum / rangeCount : 1f;
        }

        public static float ScoreVowels(this PronnunciationData a, PronnunciationData b)
        {
            var larger = a.SyllableCount > b.SyllableCount ? a : b;
            var alt = a.SyllableCount > b.SyllableCount ? b : a;
            var vowelCount = larger.SyllableCount;
            var altOffset = larger.SyllableCount - alt.SyllableCount;
            var sum = 0f;
            for (int i = altOffset; i < vowelCount; i++)
                sum += larger.GetVowelBridge(i).ScoreRhyme(alt.GetVowelBridge(i - altOffset));
            return vowelCount > 0 ? sum / vowelCount : 1f;
        }

        public static float ScoreStresses(this PronnunciationData a, PronnunciationData b)
        {
            var larger = a.SyllableCount > b.SyllableCount ? a : b;
            var alt = a.SyllableCount > b.SyllableCount ? b : a;
            var stressCount = larger.SyllableCount;
            var altOffset = larger.SyllableCount - alt.SyllableCount;
            var sum = 0f;
            for (int i = altOffset; i < stressCount; i++)
                sum += 1f - (Math.Abs(larger.GetSyllableStress(i) - alt.GetSyllableStress(i - altOffset)) / (float)SyllableStress.Primary);
            return stressCount > 0 ? sum / stressCount : 1f;
        }

        private static float ScoreRhyme(ReadOnlySpan<ISemiSyllable> a, ReadOnlySpan<ISemiSyllable> b)
        {
            var larger = a.Length > b.Length ? a : b;
            var alt = a.Length > b.Length ? b : a;
            var sum = 0f;
            for (int i = 0; i < larger.Length; i++)
            {
                var phonym = larger[i];
                var largestMatch = 0f;
                for (int j = 0; j < alt.Length; j++)
                    largestMatch = Math.Max(largestMatch, phonym.ScoreRhyme(alt[j]));
                sum += largestMatch;
            }
            return larger.Length > 0 ? sum / larger.Length : 1f;
        }

        private enum ConsonantSide
        {
            Prefixed,
            Postfixed
        }
    }

    public enum RhymeAlignment
    {
        Begin,
        End
    }
}
