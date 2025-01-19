using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class SlantGrid : ISlantGrid
    {
        public SlantGrid()
        {

        }

        private float _MaxDist = float.NaN;

        public float RhymeScoreFromDistance(float dist)
        {
            if (_MaxDist == float.NaN)
                throw new InvalidOperationException("No max distance is set");
            return 1f - (dist / _MaxDist);
        }

        public void UpdateMaxDistance(IEnumerable<IMonoPhonym> phonyms)
        {
            var coords = phonyms
                .Select(p => p.SlantCoords);
            _MaxDist = coords
                .SelectMany(a => coords
                .Select(b => a.GetDistance(b)))
                .Max();
        }
    }
}
