using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class InputBar : ConsoleUIElement
    {
        public InputBar() 
        {
            
        }

        public async Task LoopReadAsync(string inputText, Action<string> callback, CancellationToken cancelToken)
        {
            while (!cancelToken.IsCancellationRequested)
            {
                _Console.RenderCursorPosition = (0, 1);
                _Console.Write(new string(' ', Console.WindowWidth), false);
                var text = await _Console.ReadLineAsync(cancelToken, true);
                if (!cancelToken.IsCancellationRequested)
                    callback(text);
            }
        }

    }
}
