using poetrain.Data;
using poetrain.Markov;
using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class ReverseRhymer
    {
        private IPhoneticDictionary _Dict;
        private IReversePhoneticDictionary _ReverseDict;
        private IPredictionTable _Markov;
        private InputLog _InputLog = new InputLog();
        private InputBar _InputBar = new InputBar();

        public ReverseRhymer(IPhoneticDictionary dict, IReversePhoneticDictionary reverseDict, IPredictionTable markov)
        {
            _Dict = dict;
            _ReverseDict = reverseDict;
            _Markov = markov;
        }

        public async Task EnterLoop(CancellationToken cancelToken)
        {
            await _InputBar.LoopReadAsync((text) =>
            {
                _InputLog.ClearLog();
                var oneWordsOnly = text.Length > 0 && text[text.Length - 1] == '*';
                if (oneWordsOnly)
                    text = text.TrimEnd('*');
                var transcriptionArray = text
                .Split(' ')
                .Select(_Dict.TryGetTranscription)
                .ToArray();
                if (transcriptionArray
                    .Where(t => t == null)
                    .Any())
                    return;
                var transcription = ITranscription.Concat(transcriptionArray!);
                var suggestionRhymes = transcription
                    .SelectMany(p => oneWordsOnly ? _ReverseDict.FindRhymes(p, false) : _ReverseDict.FindRhymes(p, _Markov))
                    .OrderByDescending(p => p.Value)
                    .Select(p => p.Key.Transcription.Word);
                //_InputLog.ShowSuggestionRhymeLists(suggestionRhymes);
            }, cancelToken, "Enter words or phrases [add * to end for one word rhymes only]");
        }
    }
}
