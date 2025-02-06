using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class ConsoleInputReader : IDisposable
    {
        public ConsoleInputReader(InputReaderState readerState, CancellationTokenSource cancelSource)
        {
            _ReaderState = readerState;
            _CancelSource = cancelSource;
        }

        private InputReaderState _ReaderState;
        private CancellationTokenSource _CancelSource;

        public async Task ReadKeysAsync(Func<ConsoleKeyInfo, bool> callback, CancellationToken cancelToken)
        {
            var state = _ReaderState;
            await Task.Run(() =>
            {
                lock (state.Lock)
                {
                    var changeCounter = state.Counter;
                    for (; ;)
                    {
                        state.CancelToken.ThrowIfCancellationRequested();
                        cancelToken.ThrowIfCancellationRequested();
                        Monitor.Wait(state.Lock, 200); // timeout is to check on cancellation token frequently
                        var updatedChangeCounter = state.Counter;
                        // the lock is reacquired, so comparison is valid
                        if (updatedChangeCounter > changeCounter)
                        {
                            var key = state.LastKey;
                            if (!callback(key))
                                break;
                        }
                        changeCounter = updatedChangeCounter;
                    }
                }
            });
        }

        public void StopReading()
        {
            _CancelSource.Cancel();
        }

        public void Dispose()
        {
            StopReading();
            _CancelSource.Dispose();
        }
    }

    public class InputReaderState
    {
        public ConsoleKeyInfo LastKey { get; set; }
        public int Counter { get; set; }
        public object Lock { get; } = new object();
        public CancellationToken CancelToken { get; }

        public InputReaderState(CancellationToken cancelToken)
        {
            CancelToken = cancelToken;
        }
    }
}
