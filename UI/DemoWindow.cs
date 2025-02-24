using poetrain.Markov;
using poetrain.Phonology;
using poetrain.UI.Forms;
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
        private IPredictionTable _Markov;
        private Random _Rand;
        private RhymeInput _Input1;
        private RhymeInput _Input2;
        private Label _Score = FormHelpers.CreateLabel(24f);
        private LinkLabel _SuggestButton = FormHelpers.CreateLinkLabel(12f);

        public DemoWindow(IPhoneticDictionary dict, IPredictionTable markov, Random rand)
        {
            this.Size = new Size(800, 600);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Text = "IPA Rhyme Demo";
            _Dict = dict;
            _Markov = markov;
            _Rand = rand;
            _Input1 = new RhymeInput(this);
            _Input2 = new RhymeInput(this);
            var xMargin = 20;
            var yOffset = 20;
            yOffset = _Input1.AddToForm(this, xMargin, yOffset);
            yOffset = _Input2.AddToForm(this, xMargin, yOffset);
            _SuggestButton.Location = new Point(xMargin, yOffset);
            _SuggestButton.LinkClicked += Suggest;
            _SuggestButton.Text = "Suggest rhyme";
            this.Controls.Add(_SuggestButton);
            yOffset += FormHelpers.GetFontHeight(12f);
            _Score.Location = new Point(xMargin, yOffset);
            _Score.Text = "Rhyme score: N/A%";
            this.Controls.Add(_Score);
        }

        private void Suggest(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            throw new NotImplementedException();
            var transcript = _Input1.Transcription;
            if (transcript == null)
                return;
            var pronnunc = transcript[_Rand.Next(transcript.PronnunciationCount)]; // choose a random pronnunciation of the first transcription to suggest a rhyme for
            //var suggestion = pronnunc.SuggestOneRhyme(_Markov, 0.05f /* uhhh idk */, 0.5f, _Rand);
            //var phrase = string.Join(' ', suggestion.Select(p => p.Transcription.Word)); // reconstruct a phrase
            //_Input2.SetText(phrase);
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
                var fontHeight = FormHelpers.GetFontHeight(12f);
                _Caption.Location = new Point(xMargin, yOffset);
                yOffset += fontHeight;
                _TextBox.Location = new Point(xMargin, yOffset);
                _TextBox.Size = new Size(_Window.ClientSize.Width - (xMargin * 2), fontHeight);
                yOffset += _TextBox.Height;
                window.Controls.Add(_Caption);
                window.Controls.Add(_TextBox);
                return yOffset;
            }

            public void SetText(string text)
            {
                _TextBox.Text = text;
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
