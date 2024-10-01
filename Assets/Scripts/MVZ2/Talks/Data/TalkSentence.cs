using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2.Talk
{
    public class TalkSentence
    {
        public string text;
        public NamespaceID speaker;
        public NamespaceID descriptionId;
        public List<NamespaceID> sounds;
        public string variant;
        public List<TalkScript> startScripts;
        public List<TalkScript> clickScripts;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("sentence");
            var textNode = document.CreateTextNode(text);
            node.AppendChild(textNode);
            node.CreateAttribute("speaker", speaker?.ToString());
            node.CreateAttribute("description", descriptionId?.ToString());
            node.CreateAttribute("sounds", sounds != null ? string.Join(";", sounds.Select(s => s.ToString())) : null);
            node.CreateAttribute("variant", variant);
            node.CreateAttribute("onStart", startScripts != null ? string.Join(";", startScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            node.CreateAttribute("onClick", clickScripts != null ? string.Join(";", clickScripts.Where(s => s != null).Select(s => s.ToString())) : null);
            return node;
        }
        public static TalkSentence FromXmlNode(XmlNode node, string defaultNsp)
        {
            var speaker = node.GetAttributeNamespaceID("speaker", defaultNsp);
            var description = node.GetAttributeNamespaceID("description", defaultNsp);
            var sounds = node.GetAttribute("sounds")?.Split(';')?.Select(s => NamespaceID.Parse(s, defaultNsp)).ToList();
            var variant = node.GetAttribute("variant");
            var startScripts = TalkScript.ParseArray(node.GetAttribute("onStart"))?.ToList();
            var clickScripts = TalkScript.ParseArray(node.GetAttribute("onClick"))?.ToList();
            var text = node.InnerText;
            return new TalkSentence()
            {
                speaker = speaker,
                descriptionId = description,
                sounds = sounds,
                variant = variant,
                text = text,
                startScripts = startScripts,
                clickScripts = clickScripts,
            };
        }
    }
}
