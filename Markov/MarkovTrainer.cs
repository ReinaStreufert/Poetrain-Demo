using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Markov
{
    public class MarkovTrainer : IMarkovTrainer
    {
        public int WindowLength { get; }

        public MarkovTrainer(int windowLen)
        {
            WindowLength = windowLen;
        }

        private Dictionary<string, Word> _WordDict = new Dictionary<string, Word>();
        private List<Word> _WordList = new List<Word>();
        private TableNode _RootNode = new TableNode();

        public void Ingest(IEnumerable<string> source)
        {
            Word?[] window = new Word?[WindowLength];
            int windowWords = 0;
            foreach (var nextWord in source.Select(GetWord))
            {
                var windowSpan = windowWords > 0 ? window.AsSpan(0, windowWords)! : Span<Word>.Empty;
                _RootNode.RecordRecursive(windowSpan, nextWord);
                // shift window
                if (windowWords > 0)
                {
                    var shiftLen = Math.Min(windowWords, WindowLength - 1);
                    for (int i = shiftLen - 1; i >= 0; i--)
                        window[i + 1] = window[i];
                }
                // add next word
                window[0] = nextWord;
                if (windowWords < WindowLength)
                    windowWords++;
            }
        }

        public async Task IngestAsync(IEnumerable<string> source)
        {
            await Task.Run(() => Ingest(source));
        }

        public IPredictionTable ToPredictionTable()
        {
            return new MarkovPredictionTable(_RootNode.ToMarkovPredictionNode(), _WordList.ToArray(), WindowLength);
        }

        private Word GetWord(string text)
        {
            if (_WordDict.TryGetValue(text, out var word))
                return word;
            else
            {
                var result = new Word(text, _WordList.Count);
                _WordList.Add(result);
                _WordDict.Add(text, result);
                return result;
            }
        }

        private class Frequency
        {
            public int Value { get; set; }

            public Frequency()
            {
                Value = 0;
            }
        }

        private class TableNode
        {
            public long SampleCount { get; set; } = 0;
            public Dictionary<Word, Frequency> NextWordFrequencies { get; } = new Dictionary<Word, Frequency>();
            public Dictionary<Word, TableNode> BackWindowNodes { get; } = new Dictionary<Word, TableNode>();

            public TableNode() { }

            public void RecordRecursive(ReadOnlySpan<Word> window, Word nextWord)
            {
                SampleCount++;
                Frequency wordFreq;
                if (!NextWordFrequencies.TryGetValue(nextWord, out wordFreq!))
                {
                    wordFreq = new Frequency();
                    NextWordFrequencies.Add(nextWord, wordFreq);
                }
                wordFreq.Value++;
                if (window.Length > 0)
                {
                    var backWord = window[0];
                    TableNode backNode;
                    if (!BackWindowNodes.TryGetValue(backWord, out backNode!))
                    {
                        backNode = new TableNode();
                        BackWindowNodes.Add(backWord, backNode);
                    }
                    var backWindow = window.Length > 1 ? window.Slice(1) : ReadOnlySpan<Word>.Empty;
                    backNode.RecordRecursive(backWindow, nextWord);
                }
            }

            public MarkovPredictionNode ToMarkovPredictionNode()
            {
                var nextWordFreqs = NextWordFrequencies
                    .Select(p => new KeyValuePair<IWord, float>(p.Key, p.Value.Value / (float)SampleCount));
                var backWindowNodes = BackWindowNodes
                    .Select(p => new KeyValuePair<IWord, MarkovPredictionNode>(p.Key, p.Value.ToMarkovPredictionNode())); // i like recursion
                return new MarkovPredictionNode(nextWordFreqs, backWindowNodes);
            }
        }


    }
}
