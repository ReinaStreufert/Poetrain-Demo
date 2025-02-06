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
                _Console.RenderCursorPosition = (0, i);
                _Console.Write(new string(' ', w), false);
            }
            _Console.RenderCursorPosition = (0, 2);
        }

        public void Log(string word, int score, int scoreFactor)
        {
            _Console.Write(word + " ");
            WriteScore(score, scoreFactor);
        }

        private void WriteScore(int score, int scoreFactor)
        {
            var unitScore = score / scoreFactor;
            var color = ConsoleColor.Red;
            if (unitScore >= 90f)
                color = ConsoleColor.Green;
            else if (unitScore >= 75)
                color = ConsoleColor.Blue;
            else if (unitScore >= 50)
                color = ConsoleColor.Yellow;
            var oldFg = FgColor;
            _Console.ForegroundColor = color;
            _Console.WriteLine($"+{score}");
            _Console.ForegroundColor = oldFg;
        }
    }
}
