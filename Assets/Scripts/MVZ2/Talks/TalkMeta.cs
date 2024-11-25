using System.Collections.Generic;
using System.Xml;

namespace MVZ2.TalkData
{
    public class TalkMeta
    {
        public List<TalkGroup> groups = new List<TalkGroup>();
        public XmlDocument ToXmlDocument()
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.AppendChild(ToXmlNode(xmlDoc));
            return xmlDoc;
        }
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("talks");
            foreach (var group in groups)
            {
                var child = group.ToXmlNode(document);
                node.AppendChild(child);
            }
            return node;
        }
        public static TalkMeta FromXmlDocument(XmlDocument document, string defaultNsp)
        {
            return TalkMeta.FromXmlNode(document["talks"], defaultNsp);
        }
        public static TalkMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var meta = new TalkMeta();
            var children = node.ChildNodes;
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                meta.groups.Add(TalkGroup.FromXmlNode(child, defaultNsp));
            }
            return meta;
        }
    }
}
