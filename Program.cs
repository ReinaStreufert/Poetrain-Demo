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
            var predictionTable = MarkovData.LoadMarkovTable(EmbeddedSource.GetEmbeddedStream("lyricsMarkov.hayley"));
            var reverseDict = ReversePhoneticDictionary.FromTranscriptions(dict, predictionTable);
            var rand = new Random();
            var englishWords = predictionTable.PredictNext() // empty window gets probabilities for all words
                .Select(p => dict.TryGetTranscription(p.Key.Text))
                .Where(t => t != null)
                .Skip(10) // skip extremely common words
                .Take(1000) // take next 1000 most common words
                .ToArray();
            var reverseRhymer = new ReverseRhymer(dict, reverseDict, predictionTable);
            await reverseRhymer.EnterLoop(CancellationToken.None);
            //var challenge = new TimeChallenge(dict, reverseDict, predictionTable, () => englishWords[rand.Next(englishWords.Length)]!);
            //await challenge.EnterChallengeLoop(CancellationToken.None);
            //Application.Run(new DemoWindow(dict, predictionTable, new Random()));
            //Console.Write("Enter a word or phrase: ")
        }
    }
}