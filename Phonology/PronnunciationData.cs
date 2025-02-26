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

        public override string ToString()
        {
            return $"{string.Concat(Body.Select(s => s.ToString()))}{string.Concat(Cap.Select(p => p.IPASymbol))}";
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

        public override string ToString()
        {
            var sb = new StringBuilder();
            if (Stress == SyllableStress.Primary)
                sb.Append("ˈ");
            else if (Stress == SyllableStress.Secondary)
                sb.Append("ˌ");
            WritePhonyms(sb, BeginConsonants);
            sb.Append(Vowel.FriendlySymbol);
            if (EndConsonant != null)
                sb.Append(EndConsonant.FriendlySymbol);
            return sb.ToString();
        }

        private void WritePhonyms(StringBuilder sb, ISemiSyllable[] phonymArr)
        {
            foreach (var phonym in phonymArr)
                sb.Append(phonym.FriendlySymbol);
        }
    }
}
