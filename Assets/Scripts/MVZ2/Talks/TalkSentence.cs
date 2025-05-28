using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.IO;
using MVZ2.Managers;
using PVZEngine;

namespace MVZ2.TalkData
{
    public class TalkSentence
    {
        public string text;
        public string description;
        public NamespaceID speaker;
        public string speakerName;
        public List<NamespaceID> sounds;
        public NamespaceID variant;
        public List<TalkScript> startScripts;
        public List<TalkScript> clickScripts;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("sentence");
            var textNode = document.CreateTextNode(text);
            node.AppendChild(textNode);
            node.CreateAttribute("speaker", speaker?.ToString());
            node.CreateAttribute("speakerName", speakerName);
            node.CreateAttribute("description", description);
            node.CreateAttribute("sounds", sounds != null ? string.Join(";", sounds.Select(s => s.ToString())) : null);
            node.CreateAttribute("variant", variant?.ToString());
            node.CreateAttribute("onStart", startScripts != null ? string.Join(";", startScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            node.CreateAttribute("onClick", clickScripts != null ? string.Join(";", clickScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            return node;
        }
        public static TalkSentence FromXmlNode(XmlNode node, string defaultNsp)
        {
            var speaker = node.GetAttributeNamespaceID("speaker", defaultNsp);
            var speakerName = node.GetAttribute("speakerName");
            var description = node.GetAttribute("description");
            var sounds = node.GetAttributeNamespaceIDArray("sounds", defaultNsp)?.ToList();
            var variant = node.GetAttributeNamespaceID("variant", defaultNsp);
            string text;
            List<TalkScript> startScripts = null;
            List<TalkScript> clickScripts = null;
            var textNode = node["text"];
            if (textNode != null)
            {
                text = textNode.InnerText;
                var startScriptNode = node["start"];
                if (startScriptNode != null)
                {
                    startScripts = TalkScript.FromArrayXmlNode(startScriptNode)?.ToList();
                }
                var clickScriptNode = node["click"];
                if (clickScriptNode != null)
                {
                    clickScripts = TalkScript.FromArrayXmlNode(clickScriptNode)?.ToList();
                }
            }
            else
            {
                text = node.InnerText;
                var startScriptStr = node.GetAttribute("onStart");
                if (!string.IsNullOrEmpty(startScriptStr))
                {
                    startScripts = TalkScript.ParseArray(startScriptStr)?.ToList();
                }
                var clickScriptStr = node.GetAttribute("onClick");
                if (!string.IsNullOrEmpty(clickScriptStr))
                {
                    clickScripts = TalkScript.ParseArray(clickScriptStr)?.ToList();
                }
            }

            return new TalkSentence()
            {
                speaker = speaker,
                speakerName = speakerName,
                description = description,
                sounds = sounds,
                variant = variant,
                text = text,
                startScripts = startScripts,
                clickScripts = clickScripts,
            };
        }
        public string GetSpeakerName(MainManager main)
        {
            if (!string.IsNullOrEmpty(speakerName))
            {
                return main.ResourceManager.GetCharacterName(speakerName);
            }
            else
            {
                return main.ResourceManager.GetCharacterName(speaker);
            }

        }
    }
}
