using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Rtf
{
    public class RTFToken : IRTFToken
    {
        private Action<IRTFWriterContext> _Action;

        public RTFToken(Action<IRTFWriterContext> action)
        {
            _Action = action;
        }

        public void WriteTo(IRTFWriterContext ctx)
        {
            _Action(ctx);
        }
    }
}
