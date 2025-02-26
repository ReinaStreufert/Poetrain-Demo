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

        public static RTFToken ForegroundColor(Color color)
        {
            return new RTFToken(ctx =>
            {
                var colorIndex = ctx.ColorTable.GetOrAddColorIndex(color);
                ctx.WriteControlWord("cf", colorIndex);
            });
        }

        public static RTFToken BackgroundColor(Color color)
        {
            return new RTFToken(ctx =>
            {
                var colorIndex = ctx.ColorTable.GetOrAddColorIndex(color);
                ctx.WriteControlWord("cb", colorIndex);
            });
        }

        public static IRTFToken Table(IRTFToken[,] cells, int columnWidthTwips)
        {
            return new RTFToken(ctx =>
            {
                // the rtf definition for a table makes me depressed
                var colCount = cells.GetLength(0);
                var rowCount = cells.GetLength(1);
                for (int row = 0; row < rowCount; row++)
                {
                    ctx.WriteControlWord(CtrlWord.TableRow);
                    for (int col = 0; col < colCount; col++)
                        ctx.WriteControlWord(CtrlWord.CellX, (col + 1) * columnWidthTwips);
                    for (int col = 0; col < colCount; col++)
                    {
                        ctx.WriteControlWord(CtrlWord.InTable);
                        ctx.Write(cells[col, row]);
                        ctx.WriteControlWord(CtrlWord.EndCell);
                    }
                    // why is the order of these ???
                    ctx.WriteControlWord(CtrlWord.EndRow);
                }
            });
        }
    }
}
