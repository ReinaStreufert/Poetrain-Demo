using poetrain.Markov;
using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace poetrain.UI.Forms
{
    public partial class RTFTests : Form
    {
        private PronnunciationLogView _LogView = new PronnunciationLogView();
        private IPhoneticDictionary _Dict;
        private IReversePhoneticDictionary _ReverseDict;
        private IPredictionTable _Markov;

        public RTFTests(IPhoneticDictionary dict, IReversePhoneticDictionary reverseDict, IPredictionTable markov)
        {
            InitializeComponent();
            _LogView.Location = new Point(0, 140);
            _Dict = dict;
            _ReverseDict = reverseDict;
            _Markov = markov;
            Controls.Add(_LogView);
        }

        public void ShowRhymes()
        {
            var text = _PhraseInputBox.Text;
            var transcriptionArray = text
                .Split(' ')
                .Select(_Dict.TryGetTranscription)
                .ToArray();
            if (transcriptionArray
                .Where(t => t == null)
                .Any())
                return;
            var transcription = ITranscription.Concat(transcriptionArray!);
            IEnumerable<IPronnunciation> rhymeList;
            if (_MultiWordCheckbox.Checked)
                rhymeList = transcription
                    .SelectMany(p => _ReverseDict.FindRhymes(p, _Markov))
                    .OrderByDescending(p => p.Value)
                    .Select(p => p.Key)
                    .Distinct();
            else rhymeList = transcription
                    .SelectMany(p => _ReverseDict.FindRhymes(p, _MatchSyllCheckbox.Checked))
                    .OrderByDescending(p => p.Value)
                    .Select(p => p.Key)
                    .Distinct();
            _LogView.SetSource(rhymeList, transcription);
        }

        private void RTFTests_SizeChanged(object sender, EventArgs e)
        {
            _PhraseInputBox.Width = ClientSize.Width - 24;
            _LogView.Size = new Size(ClientSize.Width, ClientSize.Height - 140);
        }

        private void _MatchSyllCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            ShowRhymes();
        }

        private void _MultiWordCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            _MatchSyllCheckbox.Enabled = !_MultiWordCheckbox.Checked;
            ShowRhymes();
        }

        private void _PhraseInputBox_TextChanged(object sender, EventArgs e)
        {
            ShowRhymes();
        }
    }
}
