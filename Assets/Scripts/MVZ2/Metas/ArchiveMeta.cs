using System.Xml;
using MVZ2.IO;

namespace MVZ2.Metas
{
    public class ArchiveTagMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int Priority { get; private set; }
        public static ArchiveTagMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var priority = node.GetAttributeInt("priority") ?? 0;
            return new ArchiveTagMeta()
            {
                ID = id,
                Name = name,
                Priority = priority,
            };
        }
    }
}
