using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class RhymeScore
    {
        public IPronnunciation[] Pronnunciations { get; }
        public float Value { get; }

        public RhymeScore(float value, params IPronnunciation[] pronnunciations)
        {
            Pronnunciations = pronnunciations;
            Value = value;
        }
    }
}
