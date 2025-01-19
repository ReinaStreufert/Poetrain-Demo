using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Data
{
    public class MarkupChars
    {
        public char TranscriptionQuote { get; }
        public char PronnunciationBreak { get; }
        public char PrimaryStress { get; }
        public char SecondaryStress { get; }

        public MarkupChars(char transcriptionQuote, char pronnunciationBreak, char primaryStress, char secondaryStress)
        {
            TranscriptionQuote = transcriptionQuote;
            PronnunciationBreak = pronnunciationBreak;
            PrimaryStress = primaryStress;
            SecondaryStress = secondaryStress;
            _Chars = $"{TranscriptionQuote}{PronnunciationBreak}{PrimaryStress}{SecondaryStress}";
        }

        private readonly string _Chars;

        public bool Contains(char c) => _Chars.Contains(c);
    }
}
