using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class IPAPhonologyData : IIPAPhonologyData
    {
        public MarkupChars Markup { get; }
        public ScoreAggregationWeights ScoreAggregation { get; }
        public IEnumerable<IPALanguage> Languages => _AlphabetDict.Values.Select(a => a.Language);

        public IPAPhonologyData(MarkupChars markup, ScoreAggregationWeights scoreAggregation, IEnumerable<KeyValuePair<string, ILocalizationPhonology>> localizations)
        {
            Markup = markup;
            ScoreAggregation = scoreAggregation;
            _AlphabetDict = localizations.ToImmutableDictionary();
        }

        private ImmutableDictionary<string, ILocalizationPhonology> _AlphabetDict;

        public ILocalizationPhonology? TryGetLocale(string languageCode)
        {
            return _AlphabetDict.TryGetValue(languageCode, out var alphabet) ? alphabet : null;
        }
    }
}
