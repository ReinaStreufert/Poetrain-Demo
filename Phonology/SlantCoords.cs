using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class SlantCoords
    {
        private const int _Dimensions = 3;

        public static SlantCoords Parse(string s)
        {
            var pairStrings = s.Split(' ');
            var vectors = new int[pairStrings.Length, _Dimensions];
            var pairIndex = 0;
            foreach (var pairStr in pairStrings)
            {
                var vecStrings = pairStr.Split(',');
                if (vecStrings.Length != _Dimensions)
                    throw new FormatException("Invalid coordinate string");
                for (int d = 0; d < _Dimensions; d++)
                    vectors[pairIndex, d] = int.Parse(vecStrings[d]);
                pairIndex++;
            }
            return new SlantCoords(vectors);
        }

        public static SlantCoords Merge(IEnumerable<SlantCoords> coordRange)
        {
            var outerCount = coordRange
                .Select(c => c._Vectors.GetLength(0))
                .Sum();
            var vectors = new int[outerCount, _Dimensions];
            var outerIndex = 0;
            foreach (var coord in coordRange)
            {
                var coordVectors = coord._Vectors;
                var innerCount = coordVectors.GetLength(0);
                for (int innerIndex = 0; innerIndex < innerCount; innerIndex++)
                for (int d = 0; d < _Dimensions; d++)
                    vectors[outerIndex + innerIndex, d] = coordVectors[innerIndex, d];
                outerIndex += innerCount;
            }
            return new SlantCoords(vectors);
        }

        public SlantCoords(int x, int y, int z)
        {
            _Vectors = new int[1, _Dimensions]
            {
                { x, y, z }
            };
        }

        private SlantCoords(int[,] vectors)
        {
            _Vectors = vectors;
        }

        private int[,] _Vectors;

        public float GetDistance(SlantCoords coords)
        {
            var pairs = EnumerateVectors();
            var paramPairs = coords.EnumerateVectors();
            return pairs
                .SelectMany(a => paramPairs
                .Select(b => GetDistance(a, b)))
                .Min();
        }

        private IEnumerable<IEnumerable<int>> EnumerateVectors()
        {
            var pairCount = _Vectors.GetLength(0);
            for (int i = 0; i < pairCount; i++)
                yield return EnumerateCoordPair(i);
        }

        private IEnumerable<int> EnumerateCoordPair(int pairIndex)
        {
            for (int i = 0; i < _Dimensions; i++)
                yield return _Vectors[pairIndex, i];
        }

        private static float GetDistance(IEnumerable<int> a, IEnumerable<int> b)
        {
            var aEnum = a.GetEnumerator();
            var bEnum = b.GetEnumerator();
            var dstSqrd = 0d;
            for (; ;)
            {
                var aEnd = aEnum.MoveNext();
                var bEnd = bEnum.MoveNext();
                if (aEnd != bEnd)
                    throw new ArgumentException("The coordinate pairs are not of equal dimension");
                if (!aEnd || !bEnd)
                    break;
                var aVec = aEnum.Current;
                var bVec = bEnum.Current;
                dstSqrd += Math.Pow(aVec - bVec, 2);
            }
            return (float)Math.Sqrt(dstSqrd);
        }
    }
}
