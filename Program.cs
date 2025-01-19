using poetrain.Data;
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
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var ipa = IPAData.ParsePhonologyData(IPAData.GetEmbeddedXml("ipaConfig.xml"));
            var provider = new PhonologyProvider(ipa);
            var dict = provider.LoadLocale("en_US");
            Application.Run(new DemoWindow(dict));
        }
    }
}