using poetrain.Data;
using poetrain.Markov;
using poetrain.Phonology;
using poetrain.UI;

namespace poetrain
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static async Task Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var ipa = IPAData.ParsePhonologyData(EmbeddedSource.GetEmbeddedXml("ipaConfig.xml"));
            var provider = new PhonologyProvider(ipa);
            var dict = provider.LoadLocale("en_US");
            var reverseDict = ReversePhoneticDictionary.FromTranscriptions(dict);
            var predictionTable = MarkovData.LoadMarkovTable(EmbeddedSource.GetEmbeddedStream("lyricsMarkov.hayley"));
            var rand = new Random();
            var englishWords = predictionTable.PredictNext() // empty window gets probabilities for all words
                .ToArray();
            var reverseRhymer = new ReverseRhymer(dict, reverseDict, predictionTable);
            await reverseRhymer.EnterLoop(CancellationToken.None);
            //var challenge = new TimeChallenge(dict, reverseDict, predictionTable, () => PickWord(englishWords, rand, dict));
            //await challenge.EnterChallengeLoop(CancellationToken.None);
            //Application.Run(new DemoWindow(dict, predictionTable, new Random()));
            //Console.Write("Enter a word or phrase: ")
        }

        private static ITranscription PickWord(KeyValuePair<IWord, float>[] englishWords, Random rand, IPhoneticDictionary dict)
        {
            const float maxProbability = 0.05f; // filter out boring challenge words like "the", "and", "for"
            for (; ;)
            {
                var index = rand.Next(englishWords.Length);
                var pair = englishWords[index];
                var probability = pair.Value; // my fancy math was useless
                if (probability > maxProbability)
                    continue;
                var word = pair.Key;
                if (rand.NextDouble() <= probability)
                    return dict.TryGetTranscription(word.Text)!;
            }
        }
    }
}