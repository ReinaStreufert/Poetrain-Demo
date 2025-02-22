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
    public class TimeChallenge
    {
        private IPhoneticDictionary _Dict;
        private IReversePhoneticDictionary _ReverseDict;
        private IPredictionTable _Markov;
        private Func<ITranscription> _ChallengeWordSource;
        private StatusBar _StatusBar = new StatusBar();
        private InputLog _InputLog = new InputLog();
        private InputBar _InputBar = new InputBar();
        private HashSet<string> _InputtedPhrases = new HashSet<string>();
        private int _DurationSeconds;
        private int _Score = 0;

        public TimeChallenge(IPhoneticDictionary dict, IReversePhoneticDictionary reverseDict, IPredictionTable markov, Func<ITranscription> challengeWordSource, int durationSeconds = 30)
        {
            _Dict = dict;
            _ReverseDict = reverseDict;
            _Markov = markov;
            _ChallengeWordSource = challengeWordSource;
            _DurationSeconds = durationSeconds;
        }

        public async Task EnterChallengeLoop(CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                _InputLog.ClearLog();
                _InputtedPhrases.Clear();
                _Score = 0;
                var cancelTokenSource = new CancellationTokenSource();
                var challengeWord = _ChallengeWordSource();
                _ = StatusCountdownAsync(challengeWord, cancelTokenSource);
                await _InputBar.LoopReadAsync((t) => HandleInput(t, challengeWord), cancelTokenSource.Token);
                _StatusBar.Draw($"Score: {_Score} / High: {Persistence.HighScore} / Press escape key to continue...");
                //_InputLog.ClearLog();
                /*var pastRhymeInputs = Persistence.GetPastRhymes(challengeWord);
                if (pastRhymeInputs != null)
                    _InputLog.ShowPastInputs(pastRhymeInputs);*/
                var multiWordSuggestions = challengeWord
                    .SelectMany(p => _ReverseDict.FindRhymes(p, _Markov))
                    .OrderByDescending(p => p.Value)
                    .Select(p => p.Key.Transcription.Word);
                var oneWordSuggestions = challengeWord
                        .SelectMany(p => _ReverseDict.FindRhymes(p, false))
                        .OrderByDescending(p => p.Value)
                        .Select(p => p.Key.Transcription.Word);
                var oneWordExactSyllables = challengeWord
                    .SelectMany(p => _ReverseDict.FindRhymes(p, true))
                    .OrderByDescending(p => p.Value)
                    .Select(p => p.Key.Transcription.Word);
                await _InputLog.ShowSuggestionListsAsync(oneWordExactSyllables, oneWordSuggestions, multiWordSuggestions);
                Persistence.Save();
            }
        }

        private async Task StatusCountdownAsync(ITranscription challengeWord, CancellationTokenSource cancelTokenSource)
        {
            await _StatusBar.CountdownAsync(challengeWord.Word, TimeSpan.FromSeconds(_DurationSeconds), () => _Score);
            cancelTokenSource.Cancel();
        }

        private void HandleInput(string text, ITranscription challengeWord)
        {
            var transcriptionArray = text
                .Split(' ')
                .Select(_Dict.TryGetTranscription)
                .ToArray();
            if (transcriptionArray
                .Where(t => t == null)
                .Any())
                return;
            var transcription = ITranscription.Concat(transcriptionArray!);
            var rhymeScore = transcription.ScoreRhyme(challengeWord);
            var challengePronnunc = rhymeScore.Pronnunciations
                .Where(p => p.Transcription == challengeWord)
                .First();
            var syllCount = challengePronnunc.SyllableCount;
            var score = (int)Math.Round(rhymeScore.Value * syllCount * 100f);
            if (_InputtedPhrases.Contains(text.ToLower()))
                score = 0; // repeat words are worthless
            else
                _InputtedPhrases.Add(text.ToLower());
            _Score += score;
            _InputLog.Log(transcription.Word, score, syllCount);
            Persistence.RecordScore(_Score);
            Persistence.RecordRhyme(challengePronnunc, text.ToLower());
        }
    }
}
