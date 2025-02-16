using poetrain.Data;
using poetrain.Markov;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public static class RhymeExtensions
    {
        private const int _MaxPredictions = 1000;

        public static RhymeScore ScoreRhyme(this ITranscription a, ITranscription b)
        {
            return a
                .SelectMany(x => b
                .Select(y => x
                .ScoreRhyme(y)))
                .MaxBy(s => s.Value) ?? throw new InvalidOperationException();
        }

        public static RhymeScore ScoreRhyme(this ITranscription a, IPronnunciation b)
        {
            return a
                .Select(p => p.ScoreRhyme(b))
                .MaxBy(s => s.Value) ?? throw new InvalidOperationException();
        }

        public static RhymeScore ScoreRhyme(this IPronnunciation a, IPronnunciation b)
        {
            var provider = a.Provider;
            if (provider != b.Provider)
                throw new InvalidOperationException("Pronnunciations are not comparable");
            var scoreVal = IPronnunciationData<SyllableData>.ScoreRhyme(provider, a.Data, b.Data);
            return new RhymeScore(scoreVal, a, b);
        }
    }

    public enum RhymeAlignment
    {
        Begin,
        End
    }
}
