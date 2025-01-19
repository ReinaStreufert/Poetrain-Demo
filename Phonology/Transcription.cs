using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class Transcription : ITranscription
    {
        public IPhonologyProvider Provider { get; }
        public string Word { get; }
        public int PronnunciationCount => _Pronnnunciations.Length;

        public IPronnunciation this[int index] => _Pronnnunciations[index];

        public Transcription(IPhonologyProvider provider, string word, ISyllable[][] pronnunciations)
        {
            Provider = provider;
            Word = word;
            _Pronnnunciations = pronnunciations
                .Select(s => new Pronnunciation(provider, this, s))
                .ToArray();
        }

        private IPronnunciation[] _Pronnnunciations;

        public override string ToString()
        {
            return string.Join(", ", _Pronnnunciations.Select(p => p.ToString()));
        }

        public IEnumerator<IPronnunciation> GetEnumerator()
        {
            return ((IEnumerable<IPronnunciation>)_Pronnnunciations).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Pronnnunciations.GetEnumerator();
        }
    }

    public class Pronnunciation : IPronnunciation
    {
        public IPhonologyProvider Provider { get; }
        public ITranscription Transcription { get; }
        public int SyllableCount => _Syllables.Length;

        public ISyllable this[int index] => _Syllables[index];

        public Pronnunciation(IPhonologyProvider provider, ITranscription transcription, ISyllable[] syllables)
        {
            Provider = provider;
            Transcription = transcription;
            _Syllables = syllables;
        }

        private ISyllable[] _Syllables;

        public IEnumerator<ISyllable> GetEnumerator()
        {
            return (IEnumerator<ISyllable>)_Syllables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Syllables.GetEnumerator();
        }

        public override string ToString()
        {
            return $"/{string.Concat(EnumerateSemiSyllables().Select(s => s.IPAString))}/";
        }

        private IEnumerable<ISemiSyllable> EnumerateSemiSyllables()
        {
            ISemiSyllable[]? lastPostfix = null;
            foreach (var syllable in _Syllables)
            {
                var prefix = syllable.PrefixConsonants;
                var postfix = syllable.PostfixConsonants;
                if (prefix != lastPostfix)
                    foreach (var semiSyll in prefix)
                        yield return semiSyll;
                yield return syllable.VowelBridge;
                foreach (var semiSyll in postfix)
                    yield return semiSyll;
                lastPostfix = postfix;
            }
        }
    }
}