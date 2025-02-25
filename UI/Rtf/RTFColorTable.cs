using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Rtf
{
    public class RTFColorTable : IRTFColorTable
    {
        private List<Color> _ColorTable = new List<Color>();
        private Dictionary<Color, int> _ColorIndexDict = new Dictionary<Color, int>();
        
        public int GetOrAddColorIndex(Color color)
        {
            if (_ColorIndexDict.TryGetValue(color, out int index)) 
                return index;
            _ColorIndexDict.Add(color, _ColorTable.Count);
            _ColorTable.Add(color);
            return index;
        }

        public void WriteTo(IRTFWriterContext ctx)
        {
            ctx.WriteGrouped(new RTFToken(ctx =>
            {
                ctx.WriteControlWord(CtrlWord.ColorTable);
                ctx.WriteDelimiter();
                foreach (var color in _ColorTable)
                {
                    ctx.WriteControlWord(CtrlWord.Red, color.R);
                    ctx.WriteControlWord(CtrlWord.Green, color.G);
                    ctx.WriteControlWord(CtrlWord.Blue, color.B);
                    ctx.WriteDelimiter();
                }
            }));
        }
    }
}
