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
        private Func<ITranscription> _ChallengeWordSource;
        private StatusBar _StatusBar = new StatusBar();
        private InputBar _InputBar = new InputBar();
        private int _DurationSeconds;

        public TimeChallenge(Func<ITranscription> challengeWordSource, int durationSeconds = 30)
        {
            _ChallengeWordSource = challengeWordSource;
            _DurationSeconds = durationSeconds;
        }
    }
}
