using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public class ConsoleContext
    {
        private static ConsoleLocks _Locks = new ConsoleLocks();

        public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;
        public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;
        public (int left, int top) RenderCursorPosition { get; set; }
        public (int left, int top) VisibleCursorPosition
        {
            get => (_Locks.VisibleCursorLeft, _Locks.VisibleCursorTop);
            set
            {
                _Locks.EnterRender(() =>
                {
                    _Locks.VisibleCursorLeft = value.left;
                    _Locks.VisibleCursorTop = value.top;
                    _Locks.RestoreVisibleCursor();
                });
            }
        }

        public bool VisibleCursorShown
        {
            get => _Locks.VisibleCursorShown;
            set
            {
                _Locks.EnterRender(() =>
                {
                    _Locks.VisibleCursorShown = value;
                    _Locks.RestoreVisibleCursor();
                });
            }
        }

        public ConsoleInputReader InputReader => _Locks.InputReader;

        public ConsoleContext()
        {

        }

        public void Write(string text, bool updateCursor = true) => Write(() => Console.Write(text), updateCursor);
        public void WriteLine(string text, bool updateCursor = true) => Write(() => Console.WriteLine(text), updateCursor);
        public void WriteLine(bool updateCursor = true) => Write(() => Console.WriteLine(), updateCursor);
        public void Clear(bool updateCursor = true) => Write(() => Console.Clear(), updateCursor);

        private void Write(Action writeAction, bool updateCursor)
        {
            _Locks.EnterRender(() =>
            {
                RestoreRenderCursor();
                writeAction();
                if (updateCursor)
                    SaveRenderCursor();
                _Locks.RestoreVisibleCursor();
            });
        }

        public async Task<string> ReadLineAsync(ConsoleInputReader reader, CancellationToken cancelToken, bool omitLineBreak = false)
        {
            var sb = new StringBuilder();
            VisibleCursorPosition = RenderCursorPosition;
            await reader.ReadKeysAsync((key) =>
            {
                if (key.Key == ConsoleKey.Enter)
                    return false;
                if (key.Key == ConsoleKey.Backspace)
                {
                    if (sb.Length == 0)
                    {
                        VisibleCursorPosition = RenderCursorPosition;
                        return true;
                    }
                    RenderCursorPosition = (RenderCursorPosition.left - 1, RenderCursorPosition.top);
                    VisibleCursorPosition = RenderCursorPosition;
                    Write(" ", false);
                    sb.Remove(sb.Length - 1, 1);
                }
                else if (!key.Modifiers.HasFlag(ConsoleModifiers.Control) && !key.Modifiers.HasFlag(ConsoleModifiers.Alt) && key.KeyChar != '\0')
                {
                    var c = key.KeyChar;
                    sb.Append(c);
                    Write(c.ToString());
                    VisibleCursorPosition = RenderCursorPosition;
                }
                return true;
            }, cancelToken);
            if (!omitLineBreak)
            {
                WriteLine();
                VisibleCursorPosition = RenderCursorPosition;
            }
            return sb.ToString();
        }

        private void RestoreRenderCursor()
        {
            Console.CursorVisible = false;
            Console.SetCursorPosition(RenderCursorPosition.left, RenderCursorPosition.top);
            Console.ForegroundColor = ForegroundColor;
            Console.BackgroundColor = BackgroundColor;
        }

        private void SaveRenderCursor()
        {
            RenderCursorPosition = (Console.CursorLeft, Console.CursorTop);
        }
    }
}
