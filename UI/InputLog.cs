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

        public void ShowPastInputs(IEnumerable<string> pastInputs)
        {
            var oldFg = FgColor;
            _Console.ForegroundColor = ConsoleColor.Magenta;
            _Console.RenderCursorPosition = (0, 1);
            _Console.Write("Past inputted rhymes:");
            _Console.ForegroundColor = oldFg;
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;
            int currentColX = 0;
            int currentY = 2;
            int currentColWidth = 0;
            foreach (var pastInput in pastInputs)
            {
                if (currentColX + pastInput.Length >= w)
                    return; // stop at edge of window, ill make it scroll or something like that eventually idk
                _Console.RenderCursorPosition = (currentColX, currentY);
                _Console.Write(pastInput, false);
                currentColWidth = Math.Max(currentColWidth, pastInput.Length);
                currentY++;
                if (currentY >= h)
                {
                    currentY = 2;
                    currentColX += currentColWidth + 2;
                    currentColWidth = 0;
                }
            }
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
