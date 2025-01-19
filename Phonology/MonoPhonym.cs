using poetrain.Data;
using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class MonoPhonym : ISemiSyllable, IMonoPhonym
    {
        public SemiSyllableType Type { get; }
        public string Name { get; }
        public string IPAString { get; }
        public SlantCoords SlantCoords { get; }

        public MonoPhonym(string name, SemiSyllableType type, string iPAString, SlantCoords slantCoords, ISlantGrid slantGrid)
        {
            Type = type;
            Name = name;
            IPAString = iPAString;
            SlantCoords = slantCoords;
            _SlantGrid = slantGrid;
        }

        private ISlantGrid _SlantGrid;

        public float ScoreRhyme(ISemiSyllable semiSyllable)
        {
            if (this == semiSyllable)
                return 1f;
            return ISemiSyllable.ScoreRhyme(semiSyllable, Type, SlantCoords);
        }

        float ISemiSyllable.ScoreRhyme(SemiSyllableType type, SlantCoords slantCoords)
        {
            var dist = SlantCoords.GetDistance(slantCoords);
            return _SlantGrid.RhymeScoreFromDistance(dist);
        }
    }
}
