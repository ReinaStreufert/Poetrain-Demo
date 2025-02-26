using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Rtf
{
    public class PronnunciationRhyme : IRTFToken
    {
        public IPronnunciation Challenge { get; }
        public IPronnunciation Rhyme { get; }

        public PronnunciationRhyme(IPronnunciation challenge, IPronnunciation rhyme)
        {
            Challenge = challenge;
            Rhyme = rhyme;
            _Provider = challenge.Provider;
            if (rhyme.Provider != _Provider)
                throw new ArgumentException("Phonology providers do not match");
        }

        private IPhonologyProvider _Provider;

        public void WriteTo(IRTFWriterContext ctx)
        {
            var challengeRhymeOffset = Challenge.SyllableCount - Rhyme.SyllableCount;
            var challengeData = Challenge.Data;
            var rhymeData = Rhyme.Data;
            ctx.Write(RichText.PlainText());
            ctx.Write(RichText.ForegroundColor(Color.White));
            ctx.Write($"{Rhyme.Transcription.Word} / ");

            for (int i = 0; i < Rhyme.SyllableCount; i++)
            {
                var rhymeSyll = rhymeData.Body[i];
                if (i + challengeRhymeOffset < 0)
                {
                    ctx.Write(RichText.ForegroundColor(Color.Gray));
                    ctx.Write(rhymeSyll.ToString());
                    continue;
                }
                var challengeSyll = challengeData.Body[i + challengeRhymeOffset];
                WriteSyllableRhyme(ctx, challengeSyll, rhymeSyll);

            }
            if (challengeData.Cap.Length == 0)
                ctx.Write(RichText.ForegroundColor(Color.Gray));
            foreach (var capPhonym in rhymeData.Cap)
            {
                if (challengeData.Cap.Length > 0)
                {
                    var capPhonymScore = challengeData.Cap
                        .Select(capPhonym.ScoreRhyme)
                        .Max();
                    ctx.Write(RichText.ForegroundColor(GetScoreColor(capPhonymScore)));
                }
                ctx.Write(capPhonym.IPAString);
            }
        }

        private void WriteSyllableRhyme(IRTFWriterContext ctx, SyllableData challengeSyllable, SyllableData rhymeSyllable)
        {
            if (rhymeSyllable.Stress != SyllableStress.Unstressed)
            {
                var stressMark = rhymeSyllable.Stress == SyllableStress.Primary ? "ˈ" : "ˈ";
                var stressScore = ISyllableData.ScoreStress(challengeSyllable, rhymeSyllable);
                ctx.Write(RichText.ForegroundColor(GetScoreColor(stressScore)));
                ctx.Write(stressMark);
            }
            if (challengeSyllable.BeginConsonants.Length == 0)
                ctx.Write(RichText.ForegroundColor(Color.Gray));
            foreach (var phonym in rhymeSyllable.BeginConsonants)
            {
                if (challengeSyllable.BeginConsonants.Length > 0)
                {
                    var phonymScore = challengeSyllable.BeginConsonants
                        .Select(phonym.ScoreRhyme)
                        .Max();
                    ctx.Write(RichText.ForegroundColor(GetScoreColor(phonymScore)));
                }
                ctx.Write(phonym.IPAString);
            }
            var vowelScore = challengeSyllable.Vowel.ScoreRhyme(rhymeSyllable.Vowel);
            ctx.Write(RichText.ForegroundColor(GetScoreColor(vowelScore)));
            ctx.Write(rhymeSyllable.Vowel.IPAString);
            if (rhymeSyllable.EndConsonant != null)
            {
                if (challengeSyllable.EndConsonant != null)
                {
                    var endConsScore = challengeSyllable.EndConsonant.ScoreRhyme(rhymeSyllable.EndConsonant);
                    ctx.Write(RichText.ForegroundColor(GetScoreColor(endConsScore)));
                }
                else ctx.Write(RichText.ForegroundColor(Color.Gray));
                ctx.Write(rhymeSyllable.EndConsonant.IPAString);
            }
        }

        private Color GetScoreColor(float score)
        {
            float redF;
            float greenF;
            if (score < 0.5f)
            {
                redF = 1f;
                greenF = score * 2f;
            } else
            {
                greenF = 1f;
                redF = (1f - score) * 2f;
            }
            return Color.FromArgb(255, (int)Math.Round(redF * 255), (int)Math.Round(greenF * 255f), 0);
        }
    }
}
