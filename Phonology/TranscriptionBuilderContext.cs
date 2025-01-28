using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public partial class TranscriptionBuilder
    {
        private class TranscriptionBuilderContext : ITranscriptionBuildContext
        {
            public IPhonologyProvider Provider { get; }

            public TranscriptionBuilderContext(IPhonologyProvider provider, List<PronnunciationData> pronnunciationList)
            {
                Provider = provider;
                _Pronnunciations = pronnunciationList;
            }

            private int _StartIndex = 0;
            private int _VowelIndex = -1;
            private List<ISemiSyllable> _Phonyms = new List<ISemiSyllable>();
            private List<SyllableRange> _SyllableRanges = new List<SyllableRange>();
            private List<PronnunciationData> _Pronnunciations;
            private SyllableStress _Stress = SyllableStress.Unstressed;
            private SyllableStress _NextStress = SyllableStress.Unstressed;

            public void Consonant(ISemiSyllable consonant)
            {
                _Phonyms.Add(consonant);
            }

            public void Vowel(ISemiSyllable vowel)
            {
                if (_VowelIndex > -1)
                {
                    _SyllableRanges.Add(new SyllableRange(_StartIndex, _VowelIndex, _Phonyms.Count - _StartIndex, _Stress));
                    _StartIndex = _VowelIndex + 1;
                }
                _VowelIndex = _Phonyms.Count;
                _Stress = _NextStress;
                _NextStress = SyllableStress.Unstressed;
                _Phonyms.Add(vowel);
            }

            public void Stress(SyllableStress stressType)
            {
                _NextStress = stressType;
            }

            public void EndPronnunciation()
            {
                if (_VowelIndex > -1)
                    _SyllableRanges.Add(new SyllableRange(_StartIndex, _VowelIndex, _Phonyms.Count - _StartIndex, _Stress));
                _Stress = SyllableStress.Unstressed;
                _NextStress = SyllableStress.Unstressed;
                _StartIndex = 0;
                _VowelIndex = -1;
                _Pronnunciations.Add(new PronnunciationData(_Phonyms.ToArray(), _SyllableRanges.ToArray()));
                _Phonyms.Clear();
                _SyllableRanges.Clear();
            }
        }
    }
}
