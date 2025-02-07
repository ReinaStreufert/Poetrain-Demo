using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace poetrain.Data
{
    public static class Persistence
    {
        private const string AppDataSubdirName = "poetrain";
        private const string StatsFileName = "stats.xml";
        private const string StatsDefaultSourceName = "statsDefault.xml";

        public static string DataDirPath
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppDataSubdirName);
                Directory.CreateDirectory(path); // does nothing if already exists
                return path;
            }
        }

        private static Dictionary<string, List<string>>? _PastRhymes;
        private static int _HighScore;
        private static bool _StatsLoaded = false;

        private static void Load()
        {
            if (_StatsLoaded)
                return;
            var statsXmlPath = Path.Combine(DataDirPath, StatsFileName);
            XmlDocument xmlDocument;
            if (File.Exists(statsXmlPath))
            {
                xmlDocument = new XmlDocument();
                xmlDocument.Load(statsXmlPath);
            } else
            {
                xmlDocument = EmbeddedSource.GetEmbeddedXml(StatsDefaultSourceName);
                xmlDocument.Save(statsXmlPath);
            }
            var statsNode = xmlDocument.GetElementsByTagName("poetrainStats")
                .OfType<XmlElement>()
                .First();
            _HighScore = int.Parse(statsNode["highscore"]!.Value!);
            var pastRhymePairs = xmlDocument.GetElementsByTagName("keyword")
                .OfType<XmlElement>()
                .Select(ParseKeywordNode);
            _PastRhymes = new Dictionary<string, List<string>>(pastRhymePairs);
            _StatsLoaded = true;
        }

        private static KeyValuePair<string, List<string>> ParseKeywordNode(XmlElement node)
        {
            var keyword = node["word"]!.Value!;
            var pastRhymes = node.GetElementsByTagName("pastRhyme")
                .OfType<XmlElement>()
                .Select(n => n["word"]!.Value!)
                .ToList();
            return new KeyValuePair<string, List<string>>(keyword, pastRhymes);
        }
    }
}
