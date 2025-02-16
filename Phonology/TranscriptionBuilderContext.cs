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

            private List<SyllableData> _CurrentPronnunc = new List<SyllableData>();
            private List<PronnunciationData> _Pronnunciations;
            private List<ISemiSyllable> _BeginConsonants = new List<ISemiSyllable>();
            private ISemiSyllable? _Vowel;
            private SyllableStress _Stress = SyllableStress.Unstressed;
            private SyllableStress _NextStress = SyllableStress.Unstressed;

            public void Consonant(ISemiSyllable consonant)
            {
                if (_Vowel == null)
                    _BeginConsonants.Add(consonant);
                else
                {
                    _CurrentPronnunc.Add(new SyllableData(_BeginConsonants.ToArray(), _Vowel, consonant, _Stress));
                    _BeginConsonants.Clear();
                    _Vowel = null;
                    _Stress = _NextStress;
                    _NextStress = SyllableStress.Unstressed;
                }
            }

            public void Vowel(ISemiSyllable vowel)
            {
                if (_Vowel != null)
                {
                    _CurrentPronnunc.Add(new SyllableData(_BeginConsonants.ToArray(), _Vowel, null, _Stress));
                    _BeginConsonants.Clear();
                    _Stress = _NextStress;
                    _NextStress = SyllableStress.Unstressed;
                }
                _Vowel = vowel;
            }

            public void Stress(SyllableStress stressType)
            {
                _NextStress = stressType;
            }

            public void EndPronnunciation()
            {
                if (_Vowel != null)
                {
                    _CurrentPronnunc.Add(new SyllableData(_BeginConsonants.ToArray(), _Vowel, null, _Stress));
                    _BeginConsonants.Clear();
                }
                _Pronnunciations.Add(new PronnunciationData(_CurrentPronnunc.ToArray(), _BeginConsonants.ToArray()));
                _BeginConsonants.Clear();
                _CurrentPronnunc.Clear();
                _Vowel = null;
                _NextStress = SyllableStress.Unstressed;
                _Stress = SyllableStress.Unstressed;
            }
        }
    }
}
