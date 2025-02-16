using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public struct PronnunciationData : IPronnunciationData<SyllableData>
    {
        public int SyllableCount => Body.Length;
        public SyllableData[] Body { get; }
        public ISemiSyllable[] Cap { get; }

        public PronnunciationData(SyllableData[] body, ISemiSyllable[] cap)
        {
            Body = body;
            Cap = cap;
        }
    }

    public struct SyllableData : ISyllableData
    {
        public ISemiSyllable[] BeginConsonants { get; }
        public ISemiSyllable Vowel { get; }
        public ISemiSyllable? EndConsonant { get; }
        public SyllableStress Stress { get; }

        public SyllableData(ISemiSyllable[] beginConsonants, ISemiSyllable vowel, ISemiSyllable? endConsonant, SyllableStress stress)
        {
            BeginConsonants = beginConsonants;
            Vowel = vowel;
            EndConsonant = endConsonant;
            Stress = stress;
        }
    }
}
