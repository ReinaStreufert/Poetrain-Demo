using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class InputLog : ConsoleUIElement
    {
        public void ClearLog()
        {
            var w = Console.WindowWidth;
            var h = Console.WindowHeight;
            for (int i = 2; i < h; i++)
            {
                _Console.RenderCursorPosition = (0, h);
                _Console.Write(new string(' ', w), false);
            }
        }

        public void Log(string word, float score)
        {
            _Console.Write(word + " ");
            WriteScore(score);
        }

        private void WriteScore(float score)
        {
            var color = ConsoleColor.Red;
            if (score > 0.9f)
                color = ConsoleColor.Green;
            else if (score > 0.75f)
                color = ConsoleColor.Blue;
            else if (score > 0.5f)
                color = ConsoleColor.Yellow;
            var oldFg = FgColor;
            _Console.ForegroundColor = color;
            _Console.WriteLine($"{Math.Round(score * 100f)}%");
            _Console.ForegroundColor = oldFg;
        }
    }
}
