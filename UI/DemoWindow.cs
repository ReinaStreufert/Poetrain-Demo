using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class DemoWindow : Form
    {
        public override Color BackColor { get => Color.White; set => base.BackColor = value; }

        private IPhoneticDictionary _Dict;
        private RhymeInput _Input1;
        private RhymeInput _Input2;
        private Label _Score = FormHelpers.CreateLabel(24);

        public DemoWindow(IPhoneticDictionary dict)
        {
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "IPA Rhyme Demo";
            _Dict = dict;
            _Input1 = new RhymeInput(this);
            _Input2 = new RhymeInput(this);
            var xMargin = 20;
            var yOffset = 20;
            yOffset = _Input1.AddToForm(this, xMargin, yOffset);
            yOffset = _Input2.AddToForm(this, xMargin, yOffset);
            _Score.Location = new Point(xMargin, yOffset);
            _Score.Text = "Rhyme score: N/A%";
            this.Controls.Add(_Score);
        }

        private void UpdateRhymeScore()
        {
            var transcript1 = _Input1.Transcription;
            var transcript2 = _Input2.Transcription;
            if (transcript1 == null || transcript2 == null)
                _Score.Text = "Rhyme score: N/A%";
            else
            {
                var rhymeScore = transcript1.ScoreRhyme(transcript2);
                _Score.Text = $"Rhyme score: {rhymeScore.Value * 100f}%";
            }
        }

        private class RhymeInput
        {
            public ITranscription? Transcription => _Transcription;

            public RhymeInput(DemoWindow window)
            {
                _Dict = window._Dict;
                _Window = window;
                _Caption.Text = "Unrecognized";
                _TextBox.TextChanged += _TextBox_TextChanged;
            }

            private Label _Caption = FormHelpers.CreateLabel(12f);
            private TextBox _TextBox = FormHelpers.CreateTextBox();
            private IPhoneticDictionary _Dict;
            private DemoWindow _Window;
            private ITranscription? _Transcription;

            public int AddToForm(Form window, int xMargin, int yOffset)
            {
                _Caption.Location = new Point(xMargin, yOffset);
                yOffset += _Caption.Height;
                _TextBox.Location = new Point(xMargin, yOffset);
                _TextBox.Size = new Size(_Window.ClientSize.Width - (xMargin * 2), _Caption.Height /*est. hopefully works*/);
                yOffset += _TextBox.Height;
                window.Controls.Add(_Caption);
                window.Controls.Add(_TextBox);
                return yOffset;
            }

            private void _TextBox_TextChanged(object? sender, EventArgs e)
            {
                var oldTranscription = _Transcription;
                _Transcription = ReadTranscription();
                if (oldTranscription != _Transcription)
                {
                    _Caption.Text = _Transcription == null ? "Unrecognized" : _Transcription.ToString();
                    _Window.UpdateRhymeScore();
                }
            }

            private ITranscription? ReadTranscription()
            {
                var transcriptionArray = _TextBox.Text
                    .Split(' ')
                    .Select(_Dict.TryGetTranscription)
                    .ToArray();
                if (transcriptionArray
                    .Where(t => t == null)
                    .Any())
                    return null;
                return ITranscription.Concat(transcriptionArray!);
            }
        }
    }
}
