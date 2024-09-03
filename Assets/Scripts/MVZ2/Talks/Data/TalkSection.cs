using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2.Talk
{
    public class TalkSection
    {
        public NamespaceID nameId;
        public List<TalkScript> skipScripts;
        public List<TalkCharacter> characters;
        public List<TalkSentence> sentences;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("section");
            node.CreateAttribute("nameID", nameId?.ToString());
            node.CreateAttribute("onSkip", skipScripts != null ? string.Join(";", skipScripts.Where(s => s != null).Select(s => s.ToString())) : null);

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
            var nameID = node.GetAttributeNamespaceID("nameID", defaultNsp);
            var skipScripts = node.GetAttribute("onSkip")?.Split(';')?.Select(s => TalkScript.Parse(s))?.ToList();

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
                nameId = nameID,
                skipScripts = skipScripts,
                characters = characters,
                sentences = sentences,
            };
        }
    }
}
