using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public struct ScoreAggregationWeights
    {
        public float Stresses { get; }
        public float BeginConsonants { get; }
        public float EndConsonant { get; }
        public float Vowels { get; }

        public ScoreAggregationWeights(float stresses, float beingConsonantsScore, float endConsonantScore, float vowel)
        {
            Stresses = stresses;
            BeginConsonants = beingConsonantsScore;
            EndConsonant = endConsonantScore;
            Vowels = vowel;
        }

        public float AggregateScores(float stressScore, float vowelScore, float beginConsonantsScore, float endConsonantScore)
        {
            return (
                (stressScore * Stresses) +
                (vowelScore * Vowels) +
                (beginConsonantsScore * BeginConsonants)) +
                (endConsonantScore * EndConsonant) /
                (Stresses + BeginConsonants + Vowels);
        }
    }
}
