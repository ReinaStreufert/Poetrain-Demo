using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public struct VowelString
    {
        public ISemiSyllable[] Vowels { get; }

        public VowelString(ISemiSyllable[] vowels)
        {
            Vowels = vowels;
        }

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            if (obj is VowelString vowelStr)
                return Vowels.SequenceEqual(vowelStr.Vowels);
            else return false;
        }

        public override int GetHashCode()
        {
            var hashCode = Vowels[0].GetHashCode();
            for (int i = 1; i < Vowels.Length; i++)
                hashCode = HashCode.Combine(hashCode, Vowels[i].GetHashCode());
            return hashCode;
        }

        public override string ToString()
        {
            return string.Join(',', Vowels
                .Select(v => v.Name));
        }
    }
}
