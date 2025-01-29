using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Markov
{
    public class MarkovPredictionTable : IPredictionTable
    {
        public int WindowLength => _WindowLen;
        public IEnumerable<IWord> Words => _Words.Values;

        public MarkovPredictionTable(MarkovPredictionNode rootNode, int winowLen)
        {
            _RootNode = rootNode;
            _WindowLen = winowLen;
        }

        private MarkovPredictionNode _RootNode;
        private ImmutableDictionary<string, IWord> _Words;
        private readonly int _WindowLen;

        public IEnumerable<KeyValuePair<IWord, float>> PredictNext(params IWord[] window) // window is newest-first, so after "how are you" with a window size of 2 you have ["you", "are"]
        {
            if (window.Length > _WindowLen)
                throw new ArgumentException(nameof(window));
            Dictionary<IWord, float> predictions = new Dictionary<IWord, float>(); // pick the best sample-set match possible for each individual word
            predictions.AddRange(_RootNode.NextWordProbabilities);

            var currentNode = _RootNode;
            foreach (var word in window)
            {
                if (currentNode.BackWindowNodes.TryGetValue(word, out var node))
                    currentNode = node;
                predictions.SetRange(currentNode.NextWordProbabilities);
            }
            return predictions.OrderByDescending(p => p.Value);
        }

        public IWord? TryGetWord(string text)
        {
            return _Words.TryGetValue(text, out var word) ? word : null;
        }
    }

    public class MarkovPredictionNode
    {
        public IImmutableDictionary<IWord, float> NextWordProbabilities { get; }
        public IImmutableDictionary<IWord, MarkovPredictionNode> BackWindowNodes { get; }

        public MarkovPredictionNode(IEnumerable<KeyValuePair<IWord, float>> nextWordProbabilities, IEnumerable<KeyValuePair<IWord, MarkovPredictionNode>> backWindowNodes)
        {
            NextWordProbabilities = nextWordProbabilities.ToImmutableDictionary();
            BackWindowNodes = backWindowNodes.ToImmutableDictionary();
        }
    }
}
