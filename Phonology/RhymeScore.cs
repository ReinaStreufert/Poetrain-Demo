using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class RhymeScore
    {
        public IPronnunciation MatchedPronnunciationA { get; }
        public IPronnunciation MatchedPronnunciationB { get; }
        public int SyllableCount { get; }
        public float Value { get; }

        public RhymeScore(IPronnunciation matchedPronnunciationA, IPronnunciation matchedPronnunciationB, int syllableCount, float value)
        {
            MatchedPronnunciationA = matchedPronnunciationA;
            MatchedPronnunciationB = matchedPronnunciationB;
            SyllableCount = syllableCount;
            Value = value;
        }
    }
}
