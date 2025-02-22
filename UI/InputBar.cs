using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class InputBar : ConsoleUIElement
    {
        public InputBar(ConsoleColor bgColor = ConsoleColor.Black, ConsoleColor fgColor = ConsoleColor.Gray)
        {
            BgColor = bgColor;
            FgColor = fgColor;
            _InputReader = _Console.InputReader;
        }

        private ConsoleInputReader _InputReader;

        public async Task LoopReadAsync(Action<string> callback, CancellationToken cancelToken, string inputText = "Enter rhymes")
        {
            _Console.VisibleCursorShown = true;
            for (; ; )
            {
                _Console.RenderCursorPosition = (0, 1);
                _Console.Write(new string(' ', Console.WindowWidth), false);
                if (cancelToken.IsCancellationRequested)
                    break;
                _Console.Write($"{inputText}: ");
                try
                {
                    var text = await _Console.ReadLineAsync(_InputReader, cancelToken, true);
                    if (!cancelToken.IsCancellationRequested)
                        callback(text);
                }
                catch (OperationCanceledException) { }
            }
            _Console.VisibleCursorShown = false;
        }

        public async Task PauseTillKeyAsync()
        {
            var cancelTokenSrc = new CancellationTokenSource();
            await _InputReader.ReadKeysAsync((key) =>
            {
                return false;
            }, cancelTokenSrc.Token);
        }
    }
}
