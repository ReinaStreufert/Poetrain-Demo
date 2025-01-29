using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Markov
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TVal>(this IDictionary<TKey, TVal> dict, IEnumerable<KeyValuePair<TKey, TVal>> pairs)
        {
            foreach (var pair in pairs)
                dict.Add(pair);
        }

        public static void SetRange<TKey, TVal>(this IDictionary<TKey, TVal> dict, IEnumerable<KeyValuePair<TKey, TVal>> pairs)
        {
            foreach (var pair in pairs)
                dict[pair.Key] = pair.Value;
        }
    }
}
