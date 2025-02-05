using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class StatusBar : ConsoleUIElement
    {
        public StatusBar(ConsoleColor bgColor = ConsoleColor.DarkMagenta, ConsoleColor fgColor = ConsoleColor.White)
        {
            BgColor = bgColor;
            FgColor = fgColor;
        }

        public async Task CountdownAsync(string statusText, TimeSpan duration)
        {
            var endTime = DateTime.Now + duration;
            while (DateTime.Now < endTime)
            {
                var timeLeft = endTime - DateTime.Now;
                Draw($"{statusText} / {timeLeft.Minutes.ToString("00")}:{timeLeft.Seconds.ToString("00")}");
                await Task.Delay(500);
            }
        }

        public void Draw(string statusText)
        {
            _Console.RenderCursorPosition = (0, 0);
            _Console.Write(new string(' ', Console.WindowWidth), false);
            _Console.Write(statusText, false);
        }
    }
}
