using poetrain.Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public static class MarkovData
    {
        public static void Save(this IPredictionTable table, Stream stream)
        {
            using (var bw = new BinaryWriter(stream))
            {
                var words = table.Words.ToArray();
                var windowLen = table.WindowLength;
                bw.Write(windowLen);
                bw.Write(words.Length);
                foreach (var word in words)
                {
                    var txt = word.Text;
                    var utf8 = Encoding.UTF8.GetBytes(txt);
                    bw.Write(utf8.Length);
                    bw.Write(utf8);
                }
                table.GetRootNode().Save(words, bw);
            }
        }

        private static void Save(this MarkovPredictionNode node, IWord[] words, BinaryWriter bw)
        {
            bw.Write(node.NextWordProbabilities.Count);
            foreach (var p in node.NextWordProbabilities)
            {
                bw.Write(p.Key.Id);
                bw.Write(p.Value);
            }
            bw.Write(node.BackWindowNodes.Count);
            foreach (var p in node.BackWindowNodes)
            {
                bw.Write(p.Key.Id);
                p.Value.Save(words, bw);
            }
        }

        public static IPredictionTable LoadMarkovTable(Stream stream)
        {
            using (BinaryReader br = new BinaryReader(stream))
            {
                var windowLen = br.ReadInt32();
                var wordCount = br.ReadInt32();
                var words = new IWord[wordCount];
                for (int i = 0; i < wordCount; i++)
                    words[i] = ReadWord(br, i);
                var rootNode = ReadMarkovNode(br, words);
                return new MarkovPredictionTable(rootNode, words, windowLen);
            }
        }

        private static IWord ReadWord(BinaryReader br, int index)
        {
            var len = br.ReadInt32();
            var utf8 = br.ReadBytes(len);
            var txt = Encoding.UTF8.GetString(utf8);
            return new Word(txt, index);
        }

        private static MarkovPredictionNode ReadMarkovNode(BinaryReader br, IWord[] words)
        {
            var nextWordProbsLen = br.ReadInt32();
            var nextWordsProbs = new KeyValuePair<IWord, float>[nextWordProbsLen];
            for (int i = 0; i < nextWordProbsLen; i++)
            {
                var word = words[br.ReadInt32()];
                var prob = br.ReadSingle();
                nextWordsProbs[i] = new KeyValuePair<IWord, float>(word, prob);
            }
            var backWindowNodesLen = br.ReadInt32();
            var backWindowNodes = new KeyValuePair<IWord, MarkovPredictionNode>[backWindowNodesLen];
            for (int i = 0; i < backWindowNodesLen; i++)
            {
                var word = words[br.ReadInt32()];
                var node = ReadMarkovNode(br, words);
                backWindowNodes[i] = new KeyValuePair<IWord, MarkovPredictionNode>(word, node);
            }
            return new MarkovPredictionNode(nextWordsProbs, backWindowNodes);
        }
    }
}
