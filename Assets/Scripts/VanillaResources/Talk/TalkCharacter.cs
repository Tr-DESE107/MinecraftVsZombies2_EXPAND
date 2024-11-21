using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Talk
{
    public class TalkCharacter
    {
        public NamespaceID id;
        public NamespaceID variant;
        public string side;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("character");
            node.CreateAttribute("id", id?.ToString());
            node.CreateAttribute("variant", variant?.ToString());
            node.CreateAttribute("side", side);
            return node;
        }
        public static TalkCharacter FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var variant = node.GetAttributeNamespaceID("variant", defaultNsp);
            var side = node.GetAttribute("side");
            return new TalkCharacter()
            {
                id = id,
                variant = variant,
                side = side
            };
        }
    }
}
