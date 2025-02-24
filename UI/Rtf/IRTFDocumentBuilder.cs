using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Rtf
{
    // i want it to show the words scored where each syllable is colored based on how well it is.
    // starting from RichTextBox in windows forms, im writing this to generate RTF that renders a table
    // of wrapping pronnunciations formatted so that each phonym is colored for how well it matches.
    public interface IRTFDocumentBuilder
    {
        public void Append(IRTFToken token);
    }

    public interface IRTFToken
    {
        public void WriteTo(IRTFWriterContext ctx);
    }

    public interface IRTFWriterContext
    {
        public void Write(string rawText);
        public void Write(IRTFToken token);
        public void Write(IEnumerable<IRTFToken> tokens);
        public void WriteControlWord(string ctrlWord);
        public void WriteControlWord(string ctrlWordPrefix, object value);
        public void WriteGrouped(IRTFToken token);
        public void WriteGrouped(IEnumerable<IRTFToken> tokens);
        public IRTFColorTable ColorTable { get; }
    }

    public interface IRTFColorTable : IRTFToken
    {
        public int GetOrAddColorIndex(Color color);
    }
}
