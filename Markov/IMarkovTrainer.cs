using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Markov
{
    public interface IMarkovTrainer
    {
        public int WindowLength { get; }
        public void Ingest(IEnumerable<string> source);
        public Task IngestAsync(IEnumerable<string> source);
        public IPredictionTable ToPredictionTable();
    }
}
