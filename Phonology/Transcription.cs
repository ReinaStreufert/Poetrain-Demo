using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class Transcription : ITranscription
    {
        public IPhonologyProvider Provider { get; }
        public IPhoneticDictionary Dictionary { get; }
        public string Word { get; }
        public int PronnunciationCount => _Pronnnunciations.Length;
        public IPronnunciation this[int index] => _Pronnnunciations[index];

        public Transcription(IPhoneticDictionary dictionary, string word, IEnumerable<PronnunciationData> pronnunciations)
        {
            Provider = dictionary.Provider;
            Dictionary = dictionary;
            Word = word;
            _Pronnnunciations = pronnunciations
                .Select(d => (IPronnunciation)new Pronnunciation(dictionary.Provider, this, d))
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
        public int SyllableCount => _Data.SyllableCount;
        public PronnunciationData Data => _Data;

        public Pronnunciation(IPhonologyProvider provider, ITranscription transcription, PronnunciationData data)
        {
            Provider = provider;
            Transcription = transcription;
            _Data = data;
        }

        private PronnunciationData _Data;

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("/");
            foreach (var syllable in _Data.Body)
                WriteSyllable(sb, syllable);
            WritePhonyms(sb, _Data.Cap);
            sb.Append("/");
            return sb.ToString();
        }

        private void WriteSyllable(StringBuilder sb, SyllableData syllable)
        {
            if (syllable.Stress == SyllableStress.Primary)
                sb.Append("ˈ");
            else if (syllable.Stress == SyllableStress.Secondary)
                sb.Append("ˌ");
            WritePhonyms(sb, syllable.BeginConsonants);
            sb.Append(syllable.Vowel.IPAString);
            if (syllable.EndConsonant != null)
                sb.Append(syllable.EndConsonant.IPAString);
        }

        private void WritePhonyms(StringBuilder sb, ISemiSyllable[] phonymArr)
        {
            foreach (var phonym in phonymArr)
                sb.Append(phonym.IPAString);
        }

        public VowelString ToVowelString()
        {
            var vowelArr = new ISemiSyllable[_Data.SyllableCount];
            for (int i = 0; i < vowelArr.Length; i++)
            {
                vowelArr[i] = _Data.Body[i].Vowel;
            }
            return new VowelString(vowelArr);
        }
    }

    public struct SyllableRange
    {
        public int StartIndex;
        public int VowelIndex;
        public int TotalCount;
        public SyllableStress Stress;

        public SyllableRange(int startIndex, int vowelIndex, int totalCount, SyllableStress stress)
        {
            StartIndex = startIndex;
            VowelIndex = vowelIndex;
            TotalCount = totalCount;
            Stress = stress;
        }
    }
}