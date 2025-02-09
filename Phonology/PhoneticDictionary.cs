using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class PhoneticDictionary : IPhoneticDictionary
    {
        public IPALanguage Language => _LocalizationPhonology.Language;
        public IPhonologyProvider Provider { get; }
        public float StressScoreAggregationWeight => _LocalizationPhonology.Config.ScoreAggregation.Stresses;

        public PhoneticDictionary(IPhonologyProvider provider, ILocalizationPhonology localizationPhonology, IEnumerable<KeyValuePair<string, string>> rawTranscriptions)
        {
            Provider = provider;
            _LocalizationPhonology = localizationPhonology;
            _RawTranscriptions = rawTranscriptions.ToImmutableDictionary();
            _TranscriptionBuilder = new TranscriptionBuilder(this);
        }

        private ILocalizationPhonology _LocalizationPhonology;
        private ImmutableDictionary<string, string> _RawTranscriptions;
        private Dictionary<string, ITranscription> _TranscriptionCache = new Dictionary<string, ITranscription>();
        private TranscriptionBuilder _TranscriptionBuilder;

        public bool ContainsWord(string word) => _RawTranscriptions.ContainsKey(word);

        public ITranscription? TryGetTranscription(string word)
        {
            if (_TranscriptionCache.TryGetValue(word, out var result))
                return result;
            if (!_RawTranscriptions.TryGetValue(word, out var rawTranscription))
                return null;
            return Transcribe(word, rawTranscription);
        }

        private ITranscription Transcribe(string word, string ipa)
        {
            _TranscriptionBuilder.AddPronnunciationsFromString(ipa, _LocalizationPhonology);
            var transcription = _TranscriptionBuilder.ToTranscription(word);
            _TranscriptionBuilder.Clear();
            _TranscriptionCache.Add(word, transcription);
            return transcription;
        }

        private IEnumerable<ITranscription> EnumerateTranscriptions()
        {
            foreach (var pair in _RawTranscriptions)
            {
                var word = pair.Key;
                var ipa = pair.Value;
                if (_TranscriptionCache.TryGetValue(word, out var transcription))
                    yield return transcription;
                else yield return Transcribe(word, ipa);
            }
        }

        public IEnumerator<ITranscription> GetEnumerator()
        {
            return EnumerateTranscriptions().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return EnumerateTranscriptions().GetEnumerator();
        }
    }
}
