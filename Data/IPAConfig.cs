using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class IPAConfig : IIPAConfig
    {
        public ScoreAggregationWeights ScoreAggregation { get; }
        public MarkupChars Markup { get; }

        public IPAConfig(ScoreAggregationWeights scoreAggregation, MarkupChars markup)
        {
            ScoreAggregation = scoreAggregation;
            Markup = markup;
        }
    }
}
