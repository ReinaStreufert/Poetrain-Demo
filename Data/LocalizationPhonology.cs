using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class LocalizationPhonology : ILocalizationPhonology
    {
        public IPALanguage Language { get; }
        public string DictionarySrcName { get; }
        public IIPAConfig Config { get; }

        public LocalizationPhonology(IPALanguage language, string dictionarySrcName, IEnumerable<KeyValuePair<string, ISemiSyllable>> phonyms, IIPAConfig config)
        {
            Language = language;
            DictionarySrcName = dictionarySrcName;
            _Phonyms = phonyms.ToImmutableDictionary();
            Config = config;
        }

        private ImmutableDictionary<string, ISemiSyllable> _Phonyms;

        public ISemiSyllable? TrySemiSyllableFromIPA(string ipa)
        {
            return _Phonyms.TryGetValue(ipa, out var phonym) ? phonym : null;
        }
    }
}
