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
        private const int ColumnWidthTwips = 3000;
        private const int Limit = 1000;

        public PronnunciationLogView()
        {
            ReadOnly = true;
        }

        private double _PxPerTwip;
        private LogSource? _Source;
        private int _ColumnCount;

        public void SetSource(IEnumerable<IPronnunciation> pronnunciationList, IPronnunciation challengeRhyme)
        {
            _Source = new LogSource(pronnunciationList, challengeRhyme);
            Update(true);
        }

        private void Update(bool forceRtfUpdate)
        {
            if (_Source == null || Parent == null)
            {
                Clear();
                return;
            }
            var oldColCount = _ColumnCount;
            _ColumnCount = (int)Math.Floor(TwipsFromPx(Width) / (double)ColumnWidthTwips);
            if (forceRtfUpdate || oldColCount != _ColumnCount)
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
            var rowCount = (int)Math.Ceiling(pronnunciations.Count() / (float)_ColumnCount);
            var tableCells = new IRTFToken[_ColumnCount, rowCount];
            var col = 0;
            var row = 0;
            foreach (var pronnunc in pronnunciations)
            {
                tableCells[col, row] = new PronnunciationRhyme(challengeRhyme, pronnunc);
                col++;
                if (col >= _ColumnCount)
                {
                    row++;
                    col = 0;
                }
            }
            var docBuilder = new RtfDocumentBuilder();
            docBuilder.Append(RichText.Table(tableCells, ColumnWidthTwips));
            Rtf = docBuilder.ToString();
        }

        protected override void OnControlAdded(ControlEventArgs e)
        {
            base.OnControlAdded(e);
            var font = Font;
            using (var g = CreateGraphics())
            {
                double sizePx = font.GetHeight();
                var pxPerPt = sizePx / font.SizeInPoints;
                _PxPerTwip = pxPerPt / 20;
            }
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Update(false);
        }

        private int TwipsFromPx(double px) => (int)Math.Round(px / _PxPerTwip);

        private class LogSource
        {
            public IEnumerable<IPronnunciation> PronnunciationList { get; }
            public IPronnunciation ChallengeRhyme { get; }

            public LogSource(IEnumerable<IPronnunciation> pronnuncationList, IPronnunciation challengeRhyme)
            {
                PronnunciationList = pronnuncationList;
                ChallengeRhyme = challengeRhyme;
            }
        }
    }
}
