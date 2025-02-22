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

        public async Task ShowSuggestionListsAsync(params IEnumerable<string>[] suggestionLists)
        {
            var oldFg = FgColor;
            _Console.ForegroundColor = ConsoleColor.Magenta;
            _Console.RenderCursorPosition = (0, 1);
            _Console.Write("Suggestions (down-up to scroll / right-left change filter / esc to continue)");
            _Console.ForegroundColor = oldFg;
            var listIndex = 0;
            var list = suggestionLists[listIndex];
            var listOffsets = new Stack<int>();
            var listOffset = 0;
            var listScreenEnd = ShowSuggestionRhymeList(list, 0);

            var reader = _Console.InputReader;
            await reader.ReadKeysAsync((keyInfo) =>
            {
                if (keyInfo.Key == ConsoleKey.Escape)
                    return false;
                if (keyInfo.Key == ConsoleKey.UpArrow && listOffsets.Count > 0)
                {
                    listOffset = listOffsets.Pop();
                    listScreenEnd = ShowSuggestionRhymeList(list, listOffset);
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    listOffsets.Push(listOffset);
                    listOffset = listScreenEnd;
                    listScreenEnd = ShowSuggestionRhymeList(list, listOffset);
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow && listIndex > 0)
                {
                    listOffsets.Clear();
                    listOffset = 0;
                    listIndex--;
                    list = suggestionLists[listIndex];
                    listScreenEnd = ShowSuggestionRhymeList(list, listOffset);
                } else if (keyInfo.Key == ConsoleKey.RightArrow && listIndex < listOffsets.Count - 1)
                {
                    listOffsets.Clear();
                    listOffset = 0;
                    listIndex++;
                    list = suggestionLists[listIndex];
                    listScreenEnd = ShowSuggestionRhymeList(list, listOffset);
                }
                return true;
            }, CancellationToken.None);
        }

        private int ShowSuggestionRhymeList(IEnumerable<string> suggestions, int offset)
        {
            ClearLog();
            int w = Console.WindowWidth;
            int h = Console.WindowHeight;
            int currentColX = 0;
            int currentY = 2;
            int currentColWidth = 0;
            int itemsPrinted = 0;
            foreach (var pastInput in suggestions.Skip(offset))
            {
                if (currentColX + pastInput.Length >= w)
                    return itemsPrinted + offset; // stop at edge of window, ill make it scroll or something like that eventually idk
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
                itemsPrinted++;
            }
            return itemsPrinted + offset;
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
