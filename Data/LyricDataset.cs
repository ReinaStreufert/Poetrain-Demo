using poetrain.Markov;
using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class LyricDataset
    {
        public static void LoadToTrainer(IMarkovTrainer trainer, Stream srcStream, IPhoneticDictionary wordValidator)
        {
            using (var reader = new CsvReader(srcStream))
            {
                var songLyricSet = reader.ReadToEnd()
                    .Skip(1) // skip header row
                    .Select(c => c[3]); // lyrics column
                foreach (var lyrics in songLyricSet)
                    trainer.Ingest(ReadWords(lyrics, wordValidator));
            }
        }

        private static IEnumerable<string> ReadWords(string text, IPhoneticDictionary wordValidator)
        {
            return text
                .ToLower()
                .Split(new[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(w => w.Trim('(', ')', ',', '\'', '.', '?', '!', '-'))
                .Where(wordValidator.ContainsWord);
        }
    }
}
