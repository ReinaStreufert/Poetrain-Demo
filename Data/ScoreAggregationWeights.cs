using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class ScoreAggregationWeights
    {
        public float Stresses { get; }
        public float Consonants { get; }
        public float Vowels { get; }

        public ScoreAggregationWeights(float stresses, float consonant, float vowel)
        {
            Stresses = stresses;
            Consonants = consonant;
            Vowels = vowel;
        }

        public float AggregateScores(float stressScore, float vowelScore, float consonantScore)
        {
            return (
                (stressScore * Stresses) +
                (vowelScore * Vowels) +
                (consonantScore * Consonants)) /
                (Stresses + Consonants + Vowels);
        }
    }
}
