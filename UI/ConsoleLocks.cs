using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class ConsoleLocks
    {
        public int VisibleCursorLeft { get; set; }
        public int VisibleCursorTop { get; set; }
        public bool VisibleCursorShown { get; set; }

        public ConsoleInputReader InputReader
        {
            get
            {
                if (_Reader != null)
                    return _Reader;
                lock (_ReaderStartLock)
                {
                    if (_Reader == null)
                        _Reader = StartConsoleReader();
                }
                return _Reader;
            }
        }

        public ConsoleLocks()
        {
            VisibleCursorTop = Console.CursorTop;
            VisibleCursorLeft = Console.CursorLeft;
            VisibleCursorShown = true;
        }

        private object _RenderLock = new object();
        private ConsoleInputReader? _Reader;
        private object _ReaderStartLock = new object();

        public void EnterRender(Action callback)
        {
            lock (_RenderLock)
                callback();
        }

        private ConsoleInputReader StartConsoleReader()
        {
            var cancelTokenSource = new CancellationTokenSource();
            var readerState = new InputReaderState(cancelTokenSource.Token);
            _ = ReaderLoopAsync(cancelTokenSource.Token, readerState);
            return new ConsoleInputReader(readerState, cancelTokenSource);
        }

        private async Task ReaderLoopAsync(CancellationToken cancelToken, InputReaderState state)
        {
            await Task.Run(() =>
            {
                while (!cancelToken.IsCancellationRequested)
                {
                    var key = Console.ReadKey(false);
                    lock (state.Lock)
                    {
                        state.LastKey = key;
                        state.Counter++;
                        Monitor.PulseAll(state.Lock);
                    }
                }
            });
        }

        public void RestoreVisibleCursor()
        {
            Console.SetCursorPosition(VisibleCursorLeft, VisibleCursorTop);
            Console.CursorVisible = VisibleCursorShown;
        }

        
    }
}
