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

        public Transcription(IPhonologyProvider provider, string word, IEnumerable<PronnunciationData> pronnunciations)
        {
            Provider = provider;
            Word = word;
            _Pronnnunciations = pronnunciations
                .Select(d => new Pronnunciation(provider, this, d))
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
        public ISemiSyllable this[int phonymIndex] => _Phonyms[phonymIndex];
        public int SyllableCount => _SyllableRanges.Length;
        public int PhonymCount => _Phonyms.Length;

        public Pronnunciation(IPhonologyProvider provider, ITranscription transcription, PronnunciationData data)
        {
            Provider = provider;
            Transcription = transcription;
            _Phonyms = data.Phonyms;
            _SyllableRanges = data.SyllableRanges;
        }

        private ISemiSyllable[] _Phonyms;
        private SyllableRange[] _SyllableRanges;

        public ReadOnlySpan<ISemiSyllable> GetConsonantRange(int consonantRangeIndex)
        {
            if (consonantRangeIndex < 0)
                return ReadOnlySpan<ISemiSyllable>.Empty;
            if (consonantRangeIndex < _SyllableRanges.Length)
            {
                var range = _SyllableRanges[consonantRangeIndex];
                var len = range.VowelIndex - range.StartIndex;
                return len > 0 ? _Phonyms.AsSpan(range.StartIndex, len) : ReadOnlySpan<ISemiSyllable>.Empty;
            }
            else if (consonantRangeIndex == _SyllableRanges.Length)
            {
                var range = _SyllableRanges[_SyllableRanges.Length - 1];
                var len = (range.StartIndex + range.TotalCount) - (range.VowelIndex + 1);
                return len > 0 ? _Phonyms.AsSpan(range.VowelIndex + 1, len) : ReadOnlySpan<ISemiSyllable>.Empty;
            }
            else
                return ReadOnlySpan<ISemiSyllable>.Empty;
        }

        public SyllableStress GetSyllableStress(int syllableIndex)
        {
            return _SyllableRanges[syllableIndex].Stress;
        }

        public ISemiSyllable GetVowelBridge(int syllableIndex)
        {
            return _Phonyms[_SyllableRanges[syllableIndex].VowelIndex];
        }

        public IEnumerator<ISemiSyllable> GetEnumerator()
        {
            return (IEnumerator<ISemiSyllable>)_Phonyms.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Phonyms.GetEnumerator();
        }

        public PronnunciationData ToPronnunciationData()
        {
            return new PronnunciationData(_Phonyms, _SyllableRanges);
        }

        public override string ToString()
        {
            var str = string.Concat(_Phonyms.Select(p => p.IPAString));
            return $"/{str}/";
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

    public struct PronnunciationData
    {
        public ISemiSyllable[] Phonyms => _Phonyms;
        public SyllableRange[] SyllableRanges => _SyllableRanges;

        public PronnunciationData(ISemiSyllable[] phonyms, SyllableRange[] syllableRanges)
        {
            _Phonyms = phonyms;
            _SyllableRanges = syllableRanges;
        }

        private ISemiSyllable[] _Phonyms;
        private SyllableRange[] _SyllableRanges;

        public static PronnunciationData Concat(PronnunciationData pronnunc, PronnunciationData concatPronnunc)
        {
            var linkEndRange = pronnunc._SyllableRanges[pronnunc._SyllableRanges.Length - 1];
            var linkStartRange = concatPronnunc._SyllableRanges[0];
            var linkStartFirstBridgeLen = linkStartRange.VowelIndex - linkStartRange.StartIndex;
            linkStartRange.StartIndex = linkEndRange.VowelIndex + 1;
            linkStartRange.TotalCount += (linkEndRange.StartIndex + linkEndRange.TotalCount) - (linkEndRange.VowelIndex + 1);
            linkStartRange.VowelIndex += pronnunc._Phonyms.Length;
            linkEndRange.TotalCount += linkStartFirstBridgeLen;
            var concatPhonyms = new ISemiSyllable[pronnunc._Phonyms.Length + concatPronnunc._Phonyms.Length];
            var concatRanges = new SyllableRange[pronnunc._SyllableRanges.Length + concatPronnunc._SyllableRanges.Length];
            for (int i = 0; i < pronnunc._Phonyms.Length; i++)
                concatPhonyms[i] = pronnunc._Phonyms[i];
            for (int i = 0; i < concatPronnunc._Phonyms.Length; i++)
                concatPhonyms[pronnunc._Phonyms.Length + i] = concatPronnunc._Phonyms[i];
            for (int i = 0; i < pronnunc._SyllableRanges.Length - 1; i++)
                concatRanges[i] = pronnunc._SyllableRanges[i];
            concatRanges[pronnunc._SyllableRanges.Length - 1] = linkEndRange;
            concatRanges[pronnunc._SyllableRanges.Length] = linkStartRange;
            for (int i = 1; i < concatPronnunc._SyllableRanges.Length; i++)
            {
                var range = concatPronnunc._SyllableRanges[i];
                range.StartIndex += pronnunc._Phonyms.Length;
                range.VowelIndex += pronnunc._Phonyms.Length;
                concatRanges[pronnunc._SyllableRanges.Length + i] = range;
            }
            return new PronnunciationData(concatPhonyms, concatRanges); // ugh i hope this works
        }
    }
}