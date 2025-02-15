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
                .GroupBy(p => p.ToVowelString())
                .Select(g => new KeyValuePair<VowelString, IPronnunciation[]>(g.Key, g.ToArray()))
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
            return _Index[vowelString]
                .Select(p => new KeyValuePair<IPronnunciation, float>(p, pronnunciation.ScoreRhyme(p).Value));
        }

        public IEnumerable<IPronnunciation> FindRhymes(VowelString vowelString)
        {
            return _Index[vowelString];
        }

        public IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation, IPredictionTable markov)
        {

        }

        private IEnumerable<KeyValuePair<IPronnunciation, float>> FindRhymes(IPronnunciation pronnunciation, IPronnunciation[] syllableSplit, IPredictionTable markov)
        {
            var rhymeLists = syllableSplit
                .Select(p => FindRhymes(p
                .ToVowelString())
                .ToArray())
                .ToArray();
            var rhymeListIndices = new int[rhymeLists.Length];

        }

        private KeyValuePair<IPronnunciation, float> 

        private bool IncrementRhymeListIndices(int[] rhymeListIndices, IPronnunciation[][] rhymeLists)
        {
            // this is just counting, but each digit is in the base of the corresponding rhyme list. go through all combinations fo'real
            for (int i = 0; i < rhymeLists.Length; i++)
            {
                var newIndex = rhymeListIndices[i] + 1;
                if (rhymeListIndices[i] >= rhymeLists[i].Length)
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
            
            var breakpoints = Math.Pow(pronnunciation.SyllableCount - 1, 2); // pow2 for number of binary combinations of a point between syllables being 0 unbroken or 1 broken
            for (int i = 0; i < breakpoints; i++)
                yield return GetSyllableSplitCombination(pronnunciation, i)
                    .ToArray();
        }

        private IEnumerable<IPronnunciation> GetSyllableSplitCombination(IPronnunciation pronnunciation, int breakpointMap)
        {
            var data = pronnunciation.Data;
            var syllableRngIndex = 0;
            var syllableRngCount = 1;
            for (int i = 0; i < pronnunciation.SyllableCount - 1; i++)
            {
                if ((breakpointMap & 1) > 0)
                {
                    yield return
                        new Pronnunciation(pronnunciation.Provider, pronnunciation.Transcription, data.Subpronnunciation(syllableRngIndex, syllableRngCount));
                    syllableRngIndex = syllableRngIndex + syllableRngCount;
                    syllableRngCount = 1;
                }
                else
                    syllableRngCount++;
            }
            yield return
                        new Pronnunciation(pronnunciation.Provider, pronnunciation.Transcription, data.Subpronnunciation(syllableRngIndex, syllableRngCount));
        }
    }
}
