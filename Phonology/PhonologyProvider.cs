using poetrain.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class PhonologyProvider : IPhonologyProvider
    {
        public IEnumerable<IPALanguage> Languages => _PhonologyData.Languages;
        public ScoreAggregationWeights ScoreAggregation => _PhonologyData.ScoreAggregation;

        public PhonologyProvider(IIPAPhonologyData phonologyData)
        {
            _PhonologyData = phonologyData;
        }

        private IIPAPhonologyData _PhonologyData;

        public IPhoneticDictionary LoadLocale(string language)
        {
            return TryLoadLocale(language) ?? throw new ArgumentException($"{nameof(language)} does not exist");
        }

        public IPhoneticDictionary? TryLoadLocale(string language)
        {
            var phonology = _PhonologyData.TryGetLocale(language);
            if (phonology == null)
                return null;
            var rawTranscriptions = IPAData.ParseRawTranscriptionData(IPAData.GetEmbeddedXml(phonology.DictionarySrcName), language);
            return new PhoneticDictionary(this, phonology, rawTranscriptions);
        }
    }
}
