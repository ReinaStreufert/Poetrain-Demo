using poetrain.Markov;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class ReversePhoneticDictionary : IReversePhoneticDictionary
    {
        public static ReversePhoneticDictionary FromTranscriptions(IEnumerable<ITranscription> transcriptions)
        {
            return new ReversePhoneticDictionary(transcriptions
                .SelectMany(t => t)
                .Where(p => p.SyllableCount > 0)
                .GroupBy(p => p.ToVowelString())
                .Select(g => new KeyValuePair<VowelString, IPronnunciation[]>(g.Key, g.ToArray()))
                .ToImmutableDictionary());
        }

        public static ReversePhoneticDictionary FromTranscriptions(IEnumerable<ITranscription> transcriptions, IPredictionTable markov)
        {
            return new ReversePhoneticDictionary(transcriptions
                .SelectMany(t => t)
                .Where(p => p.SyllableCount > 0)
                .Select(p => (p, markov.TryGetWord(p.Transcription.Word)))
                .Where(w => w.Item2 != null)
                .GroupBy(w => w.p.ToVowelString())
                .Select(g => new KeyValuePair<VowelString, IPronnunciation[]>(g.Key, g
                .OrderByDescending(w => markov.GetProbability(ReadOnlySpan<IWord>.Empty, w.Item2))
                .Select(w => w.p)
                .ToArray()))
                .ToImmutableDictionary());
        }

        private ReversePhoneticDictionary(ImmutableDictionary<VowelString, IPronnunciation[]> index)
        {
            _Index = index;
        }

        private ImmutableDictionary<VowelString, IPronnunciation[]> _Index;

        public IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation)
        {
            var vowelString = pronnunciation.ToVowelString();
            return FindRhymes(vowelString)
                .Select(p => new KeyValuePair<IPronnunciation, float>(p, pronnunciation.ScoreRhyme(p).Value))
                .GroupBy(p => p.Key.Transcription.Word)
                .Select(g => g.MaxBy(p => p.Value));
        }

        public IEnumerable<IPronnunciation> FindRhymes(VowelString vowelString)
        {
            return _Index.TryGetValue(vowelString, out var result) ? result : Enumerable.Empty<IPronnunciation>();
        }

        public IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation, IPredictionTable markov)
        {
            return GetSyllableSplitCombinations(pronnunciation)
                .SelectMany(s => FindRhymes(pronnunciation, s, markov))
                .GroupBy(p => p.Key.Transcription.Word)
                .Select(g => g.MaxBy(p => p.Value));
        }

        private IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation, IPronnunciation[] syllableSplit, IPredictionTable markov)
        {
            var rhymeLists = syllableSplit
                .Select(p => FindRhymes(p
                .ToVowelString())
                .Take(50) // consider only the top 200 words for optimization
                .ToArray())
                .ToArray();
            if (rhymeLists
                .Where(l => l.Length == 0)
                .Any())
                yield break;
            var rhymeListIndices = new int[rhymeLists.Length];
            do yield return Score(pronnunciation, rhymeListIndices, rhymeLists, markov);
            while (IncrementRhymeListIndices(rhymeListIndices, rhymeLists));
        }

        private KeyValuePair<IPronnunciation, float> Score(IPronnunciation pronnunciation, int[] rhymeListIndices, IPronnunciation[][] rhymeLists, IPredictionTable markov)
        {
            IPronnunciation? rhymePronnunc = null;
            var predictWindow = new PredictionWindow(markov.WindowLength);
            var probabilityMin = float.MaxValue;
            for (int i = 0; i < rhymeLists.Length; i++)
            {
                var rhymeList = rhymeLists[i];
                var rhymeListIndex = rhymeListIndices[i];
                var rhyme = rhymeList[rhymeListIndex];
                if (rhymePronnunc == null)
                    rhymePronnunc = rhyme;
                else
                    rhymePronnunc = IPronnunciation.Concat(rhymePronnunc, rhyme);
                var rhymeWord = markov.TryGetWord(rhyme.Transcription.Word);
                if (rhymeWord != null)
                    probabilityMin = Math.Min(probabilityMin, markov.GetProbability(predictWindow.Words, rhymeWord));
                predictWindow.Push(rhymeWord!); // there is no way i forgot to add this omg
            }
            //var probabilityAvg = probabilityMin / rhymeLists.Length;
            var rhymeScore = pronnunciation.ScoreRhyme(rhymePronnunc!).Value;
            return new KeyValuePair<IPronnunciation, float>(rhymePronnunc!, rhymeScore * probabilityMin);
        }

        private bool IncrementRhymeListIndices(int[] rhymeListIndices, IPronnunciation[][] rhymeLists)
        {
            // this is just counting, but each digit is in the base of the corresponding rhyme list. go through all combinations fo'real
            for (int i = 0; i < rhymeLists.Length; i++)
            {
                var newIndex = rhymeListIndices[i] + 1;
                if (newIndex >= rhymeLists[i].Length)
                    rhymeListIndices[i] = 0;
                else
                {
                    rhymeListIndices[i] = newIndex;
                    return true;
                }
            }
            return false; // overflow aka all combinations have been exhausted
        }

        private IEnumerable<IPronnunciation[]> GetSyllableSplitCombinations(IPronnunciation pronnunciation)
        {
            var breakpoints = Math.Pow(2, pronnunciation.SyllableCount - 1); // omg not pow2, 2 to the pow of.
            for (int i = 0; i < breakpoints; i++)
                yield return GetSyllableSplitCombination(pronnunciation, i)
                    .ToArray();
        }

        private IEnumerable<IPronnunciation> GetSyllableSplitCombination(IPronnunciation pronnunciation, int breakpointMap)
        {
            var data = pronnunciation.Data;
            var syllableRngIndex = 0;
            var syllableRngCount = 1;
            for (int i = 1; i < pronnunciation.SyllableCount; i++)
            {
                if ((breakpointMap & 1) > 0)
                {
                    yield return
                        new Pronnunciation(pronnunciation.Provider, pronnunciation.Transcription, IPronnunciationData<SyllableData>.GetSubRange(pronnunciation.Data, syllableRngIndex, syllableRngCount));
                    syllableRngIndex = i;
                    syllableRngCount = 1;
                }
                else
                    syllableRngCount++;
                breakpointMap >>= 1; // super important line that i forgot [sob]
            }
            yield return
                        new Pronnunciation(pronnunciation.Provider, pronnunciation.Transcription, IPronnunciationData<SyllableData>.GetSubRange(pronnunciation.Data, syllableRngIndex, syllableRngCount));
        }
    }
}
