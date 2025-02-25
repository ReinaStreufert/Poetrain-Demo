using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.UI.Rtf
{
    public class RtfDocumentBuilder : IRTFDocumentBuilder
    {
        public RtfDocumentBuilder()
        {
            _WriterContext = new WriterContext(_Body, _ColorTable);
        }

        private List<Action<StringBuilder>> _Body = new List<Action<StringBuilder>>();
        private WriterContext _WriterContext;
        private RTFColorTable _ColorTable = new RTFColorTable();

        public void Append(IRTFToken token)
        {
            _WriterContext.Write(token);
        }

        public override string ToString()
        {
            var docActions = new List<Action<StringBuilder>>();
            var docWriter = new WriterContext(docActions, _ColorTable);
            docWriter.WriteControlWord(CtrlWord.RtfVer);
            docWriter.WriteControlWord(CtrlWord.AnsiCharset);
            docWriter.WriteControlWord(CtrlWord.DefaultFont);
            docWriter.Write(_ColorTable);
            docActions.AddRange(_Body);
            var sb = new StringBuilder();
            sb.Append("{");
            foreach (var action in docActions)
                action(sb);
            sb.Append("}");
            return sb.ToString();
        }

        private class WriterContext : IRTFWriterContext
        {
            public IRTFColorTable ColorTable { get; }

            public WriterContext(List<Action<StringBuilder>> dst, IRTFColorTable colorTable)
            {
                _Dst = dst;
                ColorTable = colorTable;
            }

            private List<Action<StringBuilder>> _Dst = new List<Action<StringBuilder>>();
            private bool _LastWasCtrlWord = false;

            public void Write(string rawText)
            {
                var requireSpace = _LastWasCtrlWord;
                _LastWasCtrlWord = false;
                _Dst.Add(sb =>
                {
                    if (requireSpace && rawText.Length > 0)
                        sb.Append(" ");
                    sb.Append(rawText
                        .Replace("\\", "\\\\")
                        .Replace("{", "\\{")
                        .Replace("}", "\\{"));
                });
            }

            public void Write(IRTFToken token)
            {
                token.WriteTo(this);
            }

            public void Write(IEnumerable<IRTFToken> tokens)
            {
                foreach (var token in tokens)
                    token.WriteTo(this);
            }

            public void WriteControlWord(string ctrlWord)
            {
                _LastWasCtrlWord = true;
                _Dst.Add(sb =>
                {
                    sb.Append($"\\{ctrlWord}");
                });
            }

            public void WriteControlWord(string ctrlWordPrefix, object value)
            {
                _LastWasCtrlWord = true;
                _Dst.Add(sb =>
                {
                    sb.Append($"\\{ctrlWordPrefix}{value.ToString()}");
                });
            }

            public void WriteGrouped(IRTFToken token)
            {
                _Dst.Add(sb =>
                {
                    sb.Append("{");
                });
                _LastWasCtrlWord = true;
                token.WriteTo(this);
                _Dst.Add(sb =>
                {
                    sb.Append("}");
                });
                _LastWasCtrlWord = true;
            }

            public void WriteGrouped(IEnumerable<IRTFToken> tokens)
            {
                _Dst.Add(sb =>
                {
                    sb.Append("{");
                });
                _LastWasCtrlWord = true;
                foreach (var token in tokens)
                    token.WriteTo(this);
                _Dst.Add(sb =>
                {
                    sb.Append("}");
                });
                _LastWasCtrlWord = true;
            }

            public void WriteDelimiter()
            {
                _LastWasCtrlWord = true;
                _Dst.Add(sb =>
                {
                    sb.Append(";");
                });
            }
        }
    }
}
