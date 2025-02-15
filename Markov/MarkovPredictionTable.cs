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
        public IEnumerable<IWord> Words => _Words;

        public MarkovPredictionTable(MarkovPredictionNode rootNode, IWord[] words, int winowLen)
        {
            _RootNode = rootNode;
            _WindowLen = winowLen;
            _Words = words;
            _WordDict = _Words.ToImmutableDictionary(w => w.Text);
        }

        private MarkovPredictionNode _RootNode;
        private ImmutableDictionary<string, IWord> _WordDict;
        private IWord[] _Words;
        private readonly int _WindowLen;

        public IEnumerable<KeyValuePair<IWord, float>> PredictNext(params IWord[] window) => PredictNext(window.AsSpan());

        public IEnumerable<KeyValuePair<IWord, float>> PredictNext(ReadOnlySpan<IWord> window) // window is newest-first, so after "how are you" with a window size of 2 you have ["you", "are"]
        {
            if (window.Length > _WindowLen)
                throw new ArgumentException(nameof(window));
            Dictionary<IWord, float> predictions = new Dictionary<IWord, float>(); // pick the best sample-set match possible for each individual word
            predictions.AddRange(_RootNode.NextWordProbabilities);

            var currentNode = _RootNode;
            foreach (var word in window)
            {
                predictions.SetRange(currentNode.NextWordProbabilities);
                if (currentNode.BackWindowNodes.TryGetValue(word, out var node))
                    currentNode = node;
                else
                    break;
            }
            return predictions
                .OrderByDescending(p => p.Value);
        }

        public float GetProbability(ReadOnlySpan<IWord> window, IWord nextWord)
        {
            var currentNode = _RootNode;
            foreach (var word in window)
            {
                if (currentNode.BackWindowNodes.TryGetValue(word, out var node))
                    currentNode = node;
                else
                    break;
            }
            return currentNode.NextWordProbabilities.TryGetValue(nextWord, out var result) ? result : 0f;
        }

        public IWord? TryGetWord(string text)
        {
            return _WordDict.TryGetValue(text, out var word) ? word : null;
        }

        public IWord GetWord(int index)
        {
            return _Words[index];
        }

        public MarkovPredictionNode GetRootNode()
        {
            return _RootNode;
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

    public class Word : IWord
    {
        public string Text { get; }
        public int Id { get; }

        public Word(string text, int id)
        {
            Text = text;
            Id = id;
        }
    }
}
