using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class CsvReader : IDisposable
    {
        public bool HasReachedEnd => _ReachedEnd;

        public CsvReader(Stream inputStream)
        {
            _Reader = new StreamReader(inputStream);
        }

        private TextReader _Reader;
        private bool _ReachedEnd = false;

        public IEnumerable<string[]> ReadToEnd()
        {
            while (!_ReachedEnd)
            {
                var row = ReadRow().ToArray();
                if (row.Length > 0)
                    yield return row;
            }
        }

        public IEnumerable<string> ReadRow()
        {
            do
            {
                var cell = ReadCell();
                if (cell == null)
                {
                    _ReachedEnd = true;
                    yield break;
                }
                yield return cell;
            } while (_Reader.Read() == ',');
            while (char.IsControl((char)_Reader.Peek()))
                _Reader.Read();
        }

        private string? ReadCell()
        {
            if (_Reader.Peek() == '"')
                return ReadQuotedCell();
            var sb = new StringBuilder();
            for (; ;)
            {
                var peek = _Reader.Peek();
                if (peek < 0)
                    return sb.Length > 0 ? sb.ToString() : null;
                if (peek == ',' || char.IsControl((char)peek))
                    return sb.ToString();
                sb.Append((char)_Reader.Read());
            }
        }

        private string? ReadQuotedCell()
        {
            if (_Reader.Read() != '"')
                throw new InvalidOperationException();
            var sb = new StringBuilder();
            for (; ;)
            {
                var c = _Reader.Read();
                if (c < 0)
                    return sb.Length > 0 ? sb.ToString() : null;
                if (c == '"')
                {
                    if (_Reader.Peek() == '"') // alt escape quote
                    {
                        _Reader.Read();
                        c = '\"';
                    }
                    else
                        return sb.ToString();
                }
                else if (c == '\\') // escape character
                    c = (char)_Reader.Read();
                sb.Append((char)c);
            }
        }

        public void Dispose()
        {
            _Reader.Dispose();
        }
    }
}
