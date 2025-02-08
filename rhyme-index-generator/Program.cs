﻿// See https://aka.ms/new-console-template for more information
using poetrain.Data;
using poetrain.Phonology;
using System.Xml;

var ipa = IPAData.ParsePhonologyData();
var provider = new PhonologyProvider(ipa);
var dict = provider.LoadLocale("en_US");
var predictionTable = MarkovData.LoadMarkovTable(EmbeddedSource.GetEmbeddedStream("lyricsMarkov.hayley"));
var englishWords = predictionTable.PredictNext()
    .Select(p => p.Key.Text)
    .ToArray();
Dictionary<string, List<string>> rhymeIndex = new Dictionary<string, List<string>>();
foreach (var word in englishWords)
{
    var transcription = dict.TryGetTranscription(word);
    if (transcription == null)
        continue;
    foreach (var pronnunc in transcription)
    {
        var vowelStr = pronnunc.ToVowelString().ToString();
        List<string> wordList;
        if (!rhymeIndex.TryGetValue(vowelStr, out wordList!))
        {
            wordList = new List<string>();
            rhymeIndex.Add(vowelStr, wordList);
        }
        wordList.Add(word);
    }
}

var outputXml = new XmlDocument();
var rhymeIndexXml = outputXml.CreateElement("rhymeIndex");
rhymeIndexXml.SetAttribute("language", "en_US");
foreach (var pair in rhymeIndex)
{
    var vowelStr = pair.Key;
    var wordList = pair.Value;
    var keyRhyme = outputXml.CreateElement("keyRhyme");
    keyRhyme.SetAttribute("syllableVowels", vowelStr);
    foreach (var word in wordList)
    {
        var rhymeItem = outputXml.CreateElement("rhymeItem");
        rhymeItem.SetAttribute("word", word);
        keyRhyme.AppendChild(rhymeItem);
    }
    rhymeIndexXml.AppendChild(keyRhyme);
}
outputXml.AppendChild(rhymeIndexXml);
outputXml.Save("en_US_index.xml");