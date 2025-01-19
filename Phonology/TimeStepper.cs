using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class TimeStepper<T> where T : class
    {
        public T[] Sequence => _Sequence;

        public TimeStepper(T[] sequence)
        {
            _Sequence = sequence;
            if (sequence.Length < 2)
                _Step = 0f;
            else
                _Step = 1f / (sequence.Length - 1);
        }

        private T[] _Sequence;
        private float _Step;
        private float _Position;
        private int _StopIndex;

        public float this[T element]
        {
            get
            {
                var phonymIndex = _Sequence
                    .Select((p, i) => (p, i))
                    .Where(pair => pair.p == element)
                    .First().i;
                return this[phonymIndex];
            }
        }

        public float this[int phonymIndex]
        {
            get
            {
                if (_Step == 0f)
                    return 1f;
                if (phonymIndex == StopIndex)
                    return 1f;
                var phonymPosition = phonymIndex * _Step;
                if (phonymIndex > _StopIndex || phonymPosition > _Position)
                    return 0f;
                return phonymPosition / _Position;
            }
        }

        public float Position
        {
            get => _Position;
            set
            {
                if (value < 0f || value > 1f)
                    throw new IndexOutOfRangeException($"Position must be between 0.0 and 1.0");
                _Position = value;
                if (_Step == 0)
                    _StopIndex = 0;
                else
                    _StopIndex = Math.Min((int)MathF.Floor(value / _Step), _Sequence.Length - 1);
            }
        }

        public int StopIndex
        {
            get => _StopIndex;
            set
            {
                if (value < 0 || value >= _Sequence.Length)
                    throw new IndexOutOfRangeException(nameof(value));
                _StopIndex = value;
                _Position = value * _Step;
            }
        }

        public static TimeStepper<T>? AdvanceClosestStop(TimeStepper<T> a, TimeStepper<T> b)
        {
            bool aEnd = a._StopIndex >= a._Sequence.Length - 1;
            bool bEnd = b._StopIndex >= b._Sequence.Length - 1;
            if (aEnd && bEnd)
                return null;
            var aNextPos = aEnd ? a._Position : a._Step * (a._StopIndex + 1);
            if (a._Sequence.Length == b._Sequence.Length)
            {
                a._StopIndex++;
                b._StopIndex = a._StopIndex;
                a._Position = aNextPos;
                b._Position = aNextPos;
                return a;
            }
            var bNextPos = bEnd ? b._Position : b._Step * (b._StopIndex + 1);
            if (bEnd || aNextPos <= bNextPos)
            {
                a._StopIndex++;
                a._Position = aNextPos;
                b.Position = aNextPos;
                return a;
            }
            else if (aEnd || bNextPos < aNextPos)
            {
                b._StopIndex++;
                b._Position = bNextPos;
                a.Position = bNextPos;
                return b;
            }
            else throw new InvalidOperationException("something makes no sense");
        }
    }
}
