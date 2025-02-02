using poetrain.Data;
using poetrain.Phonology;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace poetrain.Phonology
{
    public partial class TranscriptionBuilder : ITranscriptionBuilder
    {
        public TranscriptionBuilder(IPhoneticDictionary dictionary)
        {
            _Dict = dictionary;
        }

        private List<PronnunciationData> _Pronnunciations = new List<PronnunciationData>();
        private IPhoneticDictionary _Dict;

        public void AddPronnunciationsFromString(string ipaString, ILocalizationPhonology alphabet)
        {
            var ctx = new TranscriptionBuilderContext(_Dict.Provider, _Pronnunciations);
            foreach (var token in alphabet.TokensFromIPAString(ipaString))
                token.WriteTo(ctx);
        }

        public ITranscription ToTranscription(string word)
        {
            return new Transcription(_Dict, word, _Pronnunciations.ToArray());
        }

        public void Clear()
        {
            _Pronnunciations.Clear();
        }
    }
}
 