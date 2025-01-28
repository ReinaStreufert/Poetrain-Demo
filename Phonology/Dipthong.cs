using poetrain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class Dipthong : ISemiSyllable
    {
        public SemiSyllableType Type => SemiSyllableType.Vowel;
        public string Name { get; }
        public string IPAString { get; }

        public Dipthong(string name, string ipaString, ISemiSyllable startPhonym, ISemiSyllable endPhonym)
        {
            Name = name;
            IPAString = ipaString;
            _StartPhonym = startPhonym;
            _EndPhonym = endPhonym;
        }

        private ISemiSyllable _StartPhonym;
        private ISemiSyllable _EndPhonym;

        public float ScoreRhyme(ISemiSyllable semiSyllable)
        {
            if (semiSyllable == this)
                return 1f;
            var score1 = _StartPhonym.ScoreRhyme(semiSyllable);
            var score2 = _EndPhonym.ScoreRhyme(semiSyllable);
            return (score1 + score2) / 2f;
        }

        float ISemiSyllable.ScoreRhyme(SemiSyllableType type, SlantCoords slantCoords)
        {
            var score1 = ISemiSyllable.ScoreRhyme(_StartPhonym, type, slantCoords);
            var score2 = ISemiSyllable.ScoreRhyme(_EndPhonym, type, slantCoords);
            return (score1 + score2) / 2f;
        }
    }
}
