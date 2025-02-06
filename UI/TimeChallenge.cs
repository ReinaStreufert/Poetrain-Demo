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
        private Func<ITranscription> _ChallengeWordSource;
        private StatusBar _StatusBar = new StatusBar();
        private InputBar _InputBar = new InputBar();
        private InputLog _InputLog = new InputLog();
        private HashSet<string> _InputtedPhrases = new HashSet<string>();
        private int _DurationSeconds;
        private int _Score = 0;

        public TimeChallenge(IPhoneticDictionary dict, Func<ITranscription> challengeWordSource, int durationSeconds = 30)
        {
            _Dict = dict;
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
                _StatusBar.Draw($"Score: {_Score} / Press any key to continue...");
                Console.ReadKey(false);
            }
        }

        private async Task StatusCountdownAsync(ITranscription challengeWord, CancellationTokenSource cancelTokenSource)
        {
            await _StatusBar.CountdownAsync(challengeWord.Word, TimeSpan.FromSeconds(_DurationSeconds), () => _DurationSeconds);
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
            var syllCount = rhymeScore.Pronnunciations
                .Where(p => p.Transcription == challengeWord)
                .First()
                .SyllableCount;
            var score = (int)Math.Round(rhymeScore.Value * syllCount * 100f);
            if (_InputtedPhrases.Contains(text.ToLower()))
                score = 0; // repeat words are worthless
            else
                _InputtedPhrases.Add(text.ToLower());
            _Score += score;
            _InputLog.Log(transcription.Word, score, syllCount);
        }
    }
}
