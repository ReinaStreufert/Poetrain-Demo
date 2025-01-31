using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace poetrain.Data
{
    public static class EmbeddedSource
    {
        private const string _SourceBaseName = "poetrain.sources";

        public static XmlDocument GetEmbeddedXml(string sourceName)
        {
            string xml;
            using (Stream manifestStream = GetEmbeddedStream(sourceName))
            using (StreamReader reader = new StreamReader(manifestStream ?? throw new ArgumentException($"parameter '{sourceName}' refers to source {sourceName} which does not exist")))
            {
                xml = reader.ReadToEnd();
            }
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            return xmlDocument;
        }

        public static Stream GetEmbeddedStream(string sourceName)
        {
            var fullSourceName = $"{_SourceBaseName}.{sourceName.Replace('/', '.')}";
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream(fullSourceName) ?? throw new ArgumentException(nameof(sourceName));
        }
    }
}
