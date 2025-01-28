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
        public TranscriptionBuilder(IPhonologyProvider provider)
        {
            _Provider = provider;
        }

        private List<PronnunciationData> _Pronnunciations = new List<PronnunciationData>();
        private IPhonologyProvider _Provider;

        public void AddPronnunciationsFromString(string ipaString, ILocalizationPhonology alphabet)
        {
            var ctx = new TranscriptionBuilderContext(_Provider, _Pronnunciations);
            foreach (var token in alphabet.TokensFromIPAString(ipaString))
                token.WriteTo(ctx);
        }

        public ITranscription ToTranscription(string word)
        {
            return new Transcription(_Provider, word, _Pronnunciations.ToArray());
        }

        public void Clear()
        {
            _Pronnunciations.Clear();
        }
    }
}
 