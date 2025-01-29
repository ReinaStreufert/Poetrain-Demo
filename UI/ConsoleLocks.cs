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

        public ConsoleLocks()
        {
            VisibleCursorTop = Console.CursorTop;
            VisibleCursorLeft = Console.CursorLeft;
            VisibleCursorShown = true;
        }

        private object _RenderLock = new object();
        private object? _ReaderStartLock;
        private Task<ConsoleKeyInfo>? _ReaderTask;

        public void EnterRender(Action callback)
        {
            lock (_RenderLock)
                callback();
        }

        public Task<ConsoleKeyInfo> ReadKeyConcurrentAsync()
        {
            var oldStartLock = Interlocked.CompareExchange(ref _ReaderStartLock, new object(), null);
            if (oldStartLock != null)
            {
                while (_ReaderTask == null)
                    lock (_ReaderStartLock) { }
                
            } else
            {
                lock (_ReaderStartLock)
                    _ReaderTask = ReadKeyAsync();
            }
            return _ReaderTask;
        }

        public void RestoreVisibleCursor()
        {
            Console.SetCursorPosition(VisibleCursorLeft, VisibleCursorTop);
            Console.CursorVisible = VisibleCursorShown;
        }

        private async Task<ConsoleKeyInfo> ReadKeyAsync()
        {
            return await Task.Run(() =>
            {
                var k = Console.ReadKey(false);
                _ReaderStartLock = null;
                _ReaderTask = null;
                return k;
            });
        }
    }
}
