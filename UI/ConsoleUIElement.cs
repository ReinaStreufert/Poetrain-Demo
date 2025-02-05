using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI
{
    public abstract class ConsoleUIElement
    {
        public ConsoleColor BgColor { get => _Console.BackgroundColor; set => _Console.ForegroundColor = value; }
        public ConsoleColor FgColor { get => _Console.ForegroundColor; set => _Console.ForegroundColor = value; }

        protected ConsoleContext _Console = new ConsoleContext();
    }
}
