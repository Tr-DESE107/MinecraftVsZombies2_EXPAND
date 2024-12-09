using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class ArchiveMetaList
    {
        public ArchiveTagMeta[] Tags { get; private set; }
        public static ArchiveMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var tagsNode = node["tags"];
            var tags = new List<ArchiveTagMeta>();
            if (tagsNode != null)
            {
                for (var i = 0; i < tagsNode.ChildNodes.Count; i++)
                {
                    var child = tagsNode.ChildNodes[i];
                    if (child.Name == "tag")
                    {
                        tags.Add(ArchiveTagMeta.FromXmlNode(child, defaultNsp));
                    }
                }
            }
            return new ArchiveMetaList()
            {
                Tags = tags.ToArray()
            };
        }
    }
}
