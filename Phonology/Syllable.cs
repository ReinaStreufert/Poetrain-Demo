using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public class Syllable : ISyllable
    {
        public IPhonologyProvider Provider { get; }
        public SyllableStress Stress { get; }
        public ISemiSyllable VowelBridge { get; }
        public ISemiSyllable[] PrefixConsonants { get; }
        public ISemiSyllable[] PostfixConsonants { get; }

        public Syllable(IPhonologyProvider provider, SyllableStress stress, ISemiSyllable[] prefixConsonants, ISemiSyllable[] postfixConsonants, ISemiSyllable vowelBridge)
        {
            Provider = provider;
            Stress = stress;
            PrefixConsonants = prefixConsonants;
            PostfixConsonants = postfixConsonants;
            VowelBridge = vowelBridge;
        }
    }
}
