using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Markov
{
    public class PredictionWindow
    {
        public int Count => _Count;
        public int Capacity => _Arr.Length;
        public Span<IWord> Words => _Arr.AsSpan(0, _Count);

        public PredictionWindow(int capacity)
        {
            _Arr = new IWord[capacity];
        }

        private IWord[] _Arr;
        private int _Count = 0;

        public void Push(IWord word)
        {
            var shiftCount = Math.Min(_Count, Capacity - 1);
            for (int i = shiftCount - 1; i >= 0; i--)
                _Arr[i + 1] = _Arr[i];
            _Arr[0] = word;
            if (_Count < Capacity)
                _Count++;
        }
    }
}
