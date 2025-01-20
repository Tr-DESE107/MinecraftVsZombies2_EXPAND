using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StoreChatMeta
    {
        public NamespaceID Sound { get; private set; }
        public string Text { get; private set; }
        public static StoreChatMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var sound = node.GetAttributeNamespaceID("sound", defaultNsp);
            var text = node.InnerText;
            return new StoreChatMeta()
            {
                Sound = sound,
                Text = text,
            };
        }
    }
}
