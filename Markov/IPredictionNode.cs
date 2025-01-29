using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Markov
{
    public interface IPredictionTable
    {
        public int WindowLength { get; }
        public IEnumerable<IWord> Words { get; }
        public IWord? TryGetWord(string text);
        public IEnumerable<KeyValuePair<IWord, float>> PredictNext(params IWord?[] window);
    }

    public interface IWord
    {
        public string Text { get; }
    }
}
 