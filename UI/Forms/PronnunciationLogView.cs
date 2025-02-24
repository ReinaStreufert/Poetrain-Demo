using poetrain.Phonology;
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

        private int TwipsFromPx(double px) => (int)Math.Round(px / _PxPerTwip);

        private class LogSource
        {
            private IEnumerable<IPronnunciation> PronnunciationList { get; }
            private IPronnunciation ChallengeRhyme { get; }

            public LogSource(IEnumerable<IPronnunciation> pronnuncationList, IPronnunciation challengeRhyme)
            {
                PronnunciationList = pronnuncationList;
                ChallengeRhyme = challengeRhyme;
            }
        }
    }
}
