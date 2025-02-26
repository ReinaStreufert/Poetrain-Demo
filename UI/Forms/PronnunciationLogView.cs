using poetrain.Phonology;
using poetrain.UI.Rtf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Forms
{
    public class PronnunciationLogView : RichTextBox
    {
        private const int ColumnWidthTwips = 4000;
        private const int Limit = 500;

        public PronnunciationLogView()
        {
            ReadOnly = true;
            BackColor = Color.Black;
        }

        private double _PxPerTwip = -1;
        private LogSource? _Source;
        private int _ColumnCount;

        public void SetSource(IEnumerable<IPronnunciation> pronnunciationList, ITranscription challengeRhyme)
        {
            _Source = new LogSource(pronnunciationList, challengeRhyme);
            UpdateUI(true);
        }

        private void UpdateUI(bool forceRtfUpdate)
        {
            if (_Source == null || Parent == null)
            {
                Clear();
                return;
            }
            if (_PxPerTwip < 0)
            {
                var font = new Font("Segoe UI", 12f, GraphicsUnit.Point);
                using (var g = CreateGraphics())
                {
                    double sizePx = font.GetHeight(g);
                    var pxPerPt = sizePx / font.SizeInPoints;
                    _PxPerTwip = pxPerPt / 20;
                }
            }
            var oldColCount = _ColumnCount;
            _ColumnCount = (int)Math.Floor(TwipsFromPx(Width) / (double)ColumnWidthTwips);
            if (_ColumnCount > 0 && (forceRtfUpdate || oldColCount != _ColumnCount))
                UpdateRtf();
        }

        private void UpdateRtf()
        {
            if (_Source == null)
                throw new InvalidOperationException();
            var challengeRhyme = _Source.ChallengeRhyme;
            var pronnunciations = _Source.PronnunciationList
                .Take(Limit)
                .ToArray();
            if (pronnunciations.Length == 0)
            {
                Clear();
                return;
            }
            var rowCount = (int)Math.Ceiling(pronnunciations.Length / (float)_ColumnCount);
            var tableCells = new IRTFToken[_ColumnCount, rowCount];
            var col = 0;
            var row = 0;
            foreach (var pronnunc in pronnunciations)
            {
                var usedChallengePronnunc = challengeRhyme
                    .MaxBy(challengePronnunc => challengePronnunc.ScoreRhyme(pronnunc).Value);
                if (usedChallengePronnunc == null)
                    continue;
                tableCells[col, row] = new PronnunciationRhyme(usedChallengePronnunc, pronnunc);
                col++;
                if (col >= _ColumnCount)
                {
                    row++;
                    col = 0;
                }
            }
            // fill rest of cells with blank space
            for (; row < rowCount; row++)
            {
                for (; col < _ColumnCount; col++)
                    tableCells[col, row] = new RTFToken(ctx => { });
                col = 0;
            }

            var docBuilder = new RtfDocumentBuilder();
            docBuilder.Append(RichText.Table(tableCells, ColumnWidthTwips));
            var generatedRtf = docBuilder.ToString();
            Rtf = generatedRtf;
            SelectAll();
            SelectionFont = new Font("Segoe UI", 12f, FontStyle.Bold, GraphicsUnit.Point);
            DeselectAll();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            UpdateUI(false);
        }

        private int TwipsFromPx(double px) => (int)Math.Round(px / _PxPerTwip);

        private class LogSource
        {
            public IEnumerable<IPronnunciation> PronnunciationList { get; }
            public ITranscription ChallengeRhyme { get; }

            public LogSource(IEnumerable<IPronnunciation> pronnuncationList, ITranscription challengeRhyme)
            {
                PronnunciationList = pronnuncationList;
                ChallengeRhyme = challengeRhyme;
            }
        }
    }
}
