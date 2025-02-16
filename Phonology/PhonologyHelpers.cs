using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public static class PhonologyHelpers
    {
        public static float ScoreSequence<T>(Func<T, T, float> scoreFunc, (T[], T[]) sequence)
        {
            return ScoreSequence(scoreFunc, sequence.Item1, sequence.Item2);
        }

        public static float ScoreSequence<T>(Func<T, T, float> scoreFunc, T[] seqA, T[] seqB)
        {
            var primarySeq = seqA.Length >= seqB.Length ? seqA : seqB;
            var altSeq = primarySeq == seqB ? seqA : seqB;
            var sum = 0f;
            for (int i = 0; i < primarySeq.Length; i++)
            {
                var primaryPos = i / (primarySeq.Length - 1f);
                primaryPos = float.IsNaN(primaryPos) ? 0f : primaryPos;
                var maxDist = primaryPos < 0f ? 1 - primaryPos : primaryPos;
                float maxScore = 0f;
                for (int j = 0; j < altSeq.Length; j++)
                {
                    var altPos = j / (altSeq.Length - 1f);
                    altPos = float.IsNaN(altPos) ? 0.5f : altPos;
                    var distScore = 1f - (Math.Abs(altPos - primaryPos) / maxDist);
                    var score = scoreFunc(primarySeq[i], altSeq[j]) * distScore;
                    maxScore = Math.Max(maxScore, score);
                }
                sum += altSeq.Length > 0 ? maxScore : 0.5f;
            }
            var avg = sum / primarySeq.Length;
            return float.IsNaN(avg) ? 1f : avg;
        }

        public static IEnumerable<ITranscriptionToken> TokensFromIPAString(this ILocalizationPhonology localization, string ipaString)
        {
            var bufferedChars = new StringBuilder();
            var markup = localization.Config.Markup;
            ISemiSyllable? bufferedPhonym = null;
            var isQuoted = false;
            for (int i = 0; i < ipaString.Length; i++)
            {
                var c = ipaString[i];
                if (char.IsWhiteSpace(c))
                    continue;
                if (markup.Contains(c))
                {
                    if (bufferedPhonym != null)
                    {
                        yield return new PhonymToken(bufferedPhonym);
                        bufferedPhonym = null;
                        bufferedChars.Clear();
                    }
                    if (c == markup.TranscriptionQuote)
                        isQuoted = !isQuoted;
                    else if (c == markup.PronnunciationBreak)
                        yield return new LambdaPhonymToken(ctx => ctx.EndPronnunciation());
                    else if (c == markup.PrimaryStress)
                        yield return new LambdaPhonymToken(ctx => ctx.Stress(SyllableStress.Primary));
                    else if (c == markup.SecondaryStress)
                        yield return new LambdaPhonymToken(ctx => ctx.Stress(SyllableStress.Secondary));
                } else
                {
                    if (!isQuoted)
                        throw new FormatException("IPA pronnunciation is unqouted");
                    bufferedChars.Append(c);
                    var currentPhonym = localization.TrySemiSyllableFromIPA(bufferedChars.ToString());
                    if (currentPhonym == null)
                    {
                        if (bufferedPhonym != null)
                            yield return new PhonymToken(bufferedPhonym);
                        bufferedChars.Remove(0, bufferedChars.Length - 1);
                        bufferedPhonym = localization.TrySemiSyllableFromIPA(bufferedChars.ToString());
                    } else
                        bufferedPhonym = currentPhonym;
                }
            }
            if (isQuoted)
                throw new FormatException("Pronnunciation quote is not closed");
            if (bufferedPhonym != null)
                yield return new PhonymToken(bufferedPhonym);
            yield return new LambdaPhonymToken(ctx => ctx.EndPronnunciation());
        }

        private class LambdaPhonymToken : ITranscriptionToken
        {
            public Action<ITranscriptionBuildContext> TokenAction;

            public LambdaPhonymToken(Action<ITranscriptionBuildContext> tokenAction)
            {
                TokenAction = tokenAction;
            }

            public void WriteTo(ITranscriptionBuildContext transcriptionBuilder)
            {
                TokenAction(transcriptionBuilder);
            }
        }

        private class PhonymToken : ITranscriptionToken
        {
            public ISemiSyllable Phonym { get; }

            public PhonymToken(ISemiSyllable phonym)
            {
                Phonym = phonym;
            }

            public void WriteTo(ITranscriptionBuildContext transcriptionBuilder)
            {
                if (Phonym.Type == SemiSyllableType.Vowel)
                    transcriptionBuilder.Vowel(Phonym);
                else
                    transcriptionBuilder.Consonant(Phonym);
            }
        }
    }
}
