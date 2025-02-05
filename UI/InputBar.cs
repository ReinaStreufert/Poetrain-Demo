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
        }

        public async Task LoopReadAsync(Action<string> callback, CancellationToken cancelToken, string inputText = "Enter rhymes")
        {
            for (; ; )
            {
                _Console.RenderCursorPosition = (0, 1);
                _Console.Write(new string(' ', Console.WindowWidth), false);
                if (cancelToken.IsCancellationRequested)
                    break;
                _Console.Write($"{inputText}: ");
                var text = await _Console.ReadLineAsync(cancelToken, true);
                if (!cancelToken.IsCancellationRequested)
                    callback(text);
            }
        }

    }
}
