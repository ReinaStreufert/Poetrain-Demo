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

            for (int i = 0; i < Rhyme.SyllableCount; i++)
            {
                var rhymeSyll = rhymeData.Body[i];
                Color textColor;
                if (i + challengeRhymeOffset < 0)
                    textColor = Color.Black;
                else
                {
                    var challengeSyll = rhymeData.Body[i + challengeRhymeOffset];
                    var rhymeScore = i < Rhyme.SyllableCount - 1 ?
                        ISyllableData.ScoreRhyme(_Provider, challengeSyll, rhymeSyll) :
                        ISyllableData.ScoreRhyme(_Provider, challengeSyll, rhymeSyll, challengeData.Cap, rhymeData.Cap);
                    textColor = GetScoreColor(rhymeScore);
                }
                ctx.Write(RichText.ForegroundColor(textColor));
                ctx.Write(rhymeSyll.ToString());
            }
            ctx.Write(string.Concat(rhymeData.Cap.Select(p => p.IPAString)));
        }

        private Color GetScoreColor(float score)
        {
            var green = (int)Math.Round(score * 255);
            var red = (int)Math.Round((1f - score) * 255);
            return Color.FromArgb(255, red, green, 0);
        }
    }
}
