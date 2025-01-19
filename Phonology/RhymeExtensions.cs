using poetrain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public static class RhymeExtensions
    {
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
            var syllableCount = Math.Min(a.SyllableCount, b.SyllableCount);
            var aOffset = a.SyllableCount - syllableCount;
            var bOffset = b.SyllableCount - syllableCount;
            var sum = 0f;
            for (int i = 0; i < syllableCount; i++)
            {
                var aSyll = a[aOffset + i];
                var bSyll = b[bOffset + i];
                var score = aSyll.ScoreRhyme(bSyll, scoreAggr);
                sum += score;
            }
            return new RhymeScore(a, b, syllableCount, sum / syllableCount);
        }

        public static float ScoreRhyme(this ISyllable a, ISyllable b, ScoreAggregationWeights aggrWeights)
        {
            var vowelScore = a.VowelBridge.ScoreRhyme(b.VowelBridge);
            var stressScore = 1f - (Math.Abs((int)a.Stress - (int)b.Stress) / (float)SyllableStress.Primary);
            var consonantScore = GetConsonantScore(a, b);
            return aggrWeights.AggregateScores(stressScore, vowelScore, consonantScore);
        }

        private static float GetConsonantScore(ISyllable a, ISyllable b)
        {
            var perpinducularScore = AggregateSides(s => GetPerpindiculars(a, b, s));
            var diagonalScore = AggregateSides(s => GetDiagonals(a, b, s)) / 2f;
            return Math.Min(1f, perpinducularScore + diagonalScore);
        }

        private static (ISemiSyllable[] a, ISemiSyllable[]) GetPerpindiculars(ISyllable a, ISyllable b, ConsonantSide side)
        {
            return side switch
            {
                ConsonantSide.Prefixed => (a.PrefixConsonants, b.PrefixConsonants),
                ConsonantSide.Postfixed => (a.PostfixConsonants, b.PostfixConsonants),
                _ => throw new NotImplementedException()
            };
        }

        private static (ISemiSyllable[], ISemiSyllable[]) GetDiagonals(ISyllable a, ISyllable b, ConsonantSide side)
        {
            return side switch
            {
                ConsonantSide.Prefixed => (a.PrefixConsonants, DuplicateReverse(b.PostfixConsonants)),
                ConsonantSide.Postfixed => (b.PrefixConsonants, DuplicateReverse(a.PostfixConsonants)),
                _ => throw new NotImplementedException()
            };
        }

        private static float AggregateSides(Func<ConsonantSide, (ISemiSyllable[] a, ISemiSyllable[] b)> pairFunc)
        {
            var prefixScore = PhonologyHelpers.ScoreSequence((ISemiSyllable x, ISemiSyllable y) => x.ScoreRhyme(y), pairFunc(ConsonantSide.Prefixed));
            var postfixScore = PhonologyHelpers.ScoreSequence((ISemiSyllable x, ISemiSyllable y) => x.ScoreRhyme(y), pairFunc(ConsonantSide.Postfixed));
            return (prefixScore + postfixScore) / 2f;
        }

        private static T[] DuplicateReverse<T>(T[] array)
        {
            var reversed = new T[array.Length];
            var lastIndex = array.Length - 1;
            for (int i = 0; i < array.Length; i++)
                reversed[lastIndex - i] = array[i];
            return reversed;
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
