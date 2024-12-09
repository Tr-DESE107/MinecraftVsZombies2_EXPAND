using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.TalkData
{
    public class TalkSection
    {
        public string archiveText;
        public bool canAutoSkip;
        public List<TalkScript> startScripts;
        public List<TalkScript> skipScripts;
        public List<TalkCharacter> characters;
        public List<TalkSentence> sentences;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("section");
            node.CreateAttribute("canAutoSkip", canAutoSkip.ToString());
            node.CreateAttribute("onStart", startScripts != null ? string.Join(";", startScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            node.CreateAttribute("onSkip", skipScripts != null ? string.Join(";", skipScripts.Where(s => s != null).Select(s => s.ToString())) : null);

            if (!string.IsNullOrEmpty(archiveText))
            {
                var textNode = document.CreateElement("text");
                textNode.InnerText = archiveText;
                node.AppendChild(textNode);
            }

            if (characters != null)
            {
                var charactersNode = document.CreateElement("characters");
                foreach (var character in characters)
                {
                    var child = character.ToXmlNode(document);
                    charactersNode.AppendChild(child);
                }
                node.AppendChild(charactersNode);
            }

            if (sentences != null)
            {
                var sentencesNode = document.CreateElement("sentences");
                foreach (var sentence in sentences)
                {
                    var child = sentence.ToXmlNode(document);
                    sentencesNode.AppendChild(child);
                }
                node.AppendChild(sentencesNode);
            }
            return node;
        }
        public static TalkSection FromXmlNode(XmlNode node, string defaultNsp)
        {
            var canAutoSkip = node.GetAttributeBool("canAutoSkip") ?? true;

            var startScripts = TalkScript.ParseArray(node.GetAttribute("onStart"))?.ToList();
            var skipScripts = TalkScript.ParseArray(node.GetAttribute("onSkip"))?.ToList();

            var textNode = node["text"];
            string archiveText = string.Empty;
            if (textNode != null)
            {
                archiveText = textNode.InnerText;
            }

            var charactersNode = node["characters"];
            List<TalkCharacter> characters = null;
            if (charactersNode != null)
            {
                var characterChildren = charactersNode.ChildNodes;
                characters = new List<TalkCharacter>();
                for (int i = 0; i < characterChildren.Count; i++)
                {
                    var child = characterChildren[i];
                    characters.Add(TalkCharacter.FromXmlNode(child, defaultNsp));
                }
            }
            var sentencesNode = node["sentences"];
            List<TalkSentence> sentences = null;
            if (sentencesNode != null)
            {
                var sentenceChildren = sentencesNode.ChildNodes;
                sentences = new List<TalkSentence>();
                for (int i = 0; i < sentenceChildren.Count; i++)
                {
                    var child = sentenceChildren[i];
                    sentences.Add(TalkSentence.FromXmlNode(child, defaultNsp));
                }
            }
            return new TalkSection()
            {
                canAutoSkip = canAutoSkip,
                archiveText = archiveText,
                startScripts = startScripts,
                skipScripts = skipScripts,
                characters = characters,
                sentences = sentences,
            };
        }
    }
}
