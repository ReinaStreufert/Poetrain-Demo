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

            public TranscriptionBuilderContext(IPhonologyProvider provider, List<ISyllable[]> pronnunciationList)
            {
                Provider = provider;
                _Pronnunciations = pronnunciationList;
            }

            private ISemiSyllable? _Vowel;
            private ISemiSyllable[] _Prefixed = _EmptyPhonymArr;
            private List<ISemiSyllable> _Consonants = new List<ISemiSyllable>();
            private List<ISyllable> _PronnunciationSyllables = new List<ISyllable>();
            private List<ISyllable[]> _Pronnunciations;
            private SyllableStress _Stress = SyllableStress.Unstressed;
            private SyllableStress _NextStress = SyllableStress.Unstressed;
            private static readonly ISemiSyllable[] _EmptyPhonymArr = new ISemiSyllable[0];

            public void Consonant(ISemiSyllable consonant)
            {
                _Consonants.Add(consonant);
            }

            public void Vowel(ISemiSyllable vowel)
            {
                var lastConsonantSeq = _Consonants.ToArray();
                if (_Vowel != null)
                    _PronnunciationSyllables.Add(new Syllable(Provider, _Stress, _Prefixed, lastConsonantSeq, _Vowel));
                _Prefixed = lastConsonantSeq;
                _Consonants.Clear();
                _Stress = _NextStress;
                _NextStress = SyllableStress.Unstressed;
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
                    var postfixedPhonyms = _Consonants.ToArray();
                    _PronnunciationSyllables.Add(new Syllable(Provider, _Stress, _Prefixed, postfixedPhonyms, _Vowel));
                }
                _Prefixed = _EmptyPhonymArr;
                _Consonants.Clear();
                _Stress = SyllableStress.Unstressed;
                _NextStress = SyllableStress.Unstressed;
                _Vowel = null;
                _Pronnunciations.Add(_PronnunciationSyllables.ToArray());
                _PronnunciationSyllables.Clear();
            }
        }
    }
}
