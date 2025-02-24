using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Rtf
{
    public static class RichText
    {
        public static IRTFToken PlainText()
        {
            return new RTFToken(ctx =>
            {
                ctx.WriteControlWord(CtrlWord.ParagraphReset);
                ctx.WriteControlWord(CtrlWord.Plain);
            });
        }

        public static IRTFToken Raw(string text)
        {
            return new RTFToken(ctx =>
            {
                ctx.Write(text);
            });
        }

        public static IRTFToken Concat(IEnumerable<IRTFToken> tokens)
        {
            return new RTFToken(ctx =>
            {
                foreach (var token in tokens)
                    ctx.Write(token);
            });
        }

        public static IRTFToken Table(IRTFToken[,] cells, int cellWidthTwips)
        {

        }
    }
}
