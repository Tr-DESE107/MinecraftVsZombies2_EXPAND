using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.TalkData
{
    public class TalkGroup
    {
        public string id;
        public NamespaceID requires;
        public NamespaceID requiresNot;
        public List<NamespaceID> tags;

        public TalkGroupArchiveInfo archive;
        public List<TalkSection> sections;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("group");
            node.CreateAttribute("id", id);
            node.CreateAttribute("requires", requires?.ToString());
            node.CreateAttribute("requiresNot", requiresNot?.ToString());
            node.CreateAttribute("tags", tags != null ? string.Join(";", tags.Select(t => t.ToString())) : null);
            var archiveNode = archive.ToXmlNode(document);
            node.AppendChild(archiveNode);
            foreach (var section in sections)
            {
                var child = section.ToXmlNode(document);
                node.AppendChild(child);
            }
            return node;
        }
        public static TalkGroup FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var requires = node.GetAttributeNamespaceID("requires", defaultNsp);
            var requiresNot = node.GetAttributeNamespaceID("requiresNot", defaultNsp);
            var tags = node.GetAttributeNamespaceIDArray("tags", defaultNsp)?.ToList();

            var children = node.ChildNodes;


            TalkGroupArchiveInfo archive = null;
            var sections = new List<TalkSection>();
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                switch (child.Name)
                {
                    case "section":
                        sections.Add(TalkSection.FromXmlNode(child, defaultNsp));
                        break;
                    case "archive":
                        archive = TalkGroupArchiveInfo.FromXmlNode(child, defaultNsp);
                        break;
                }
            }
            if (archive == null)
            {
                archive = new TalkGroupArchiveInfo();
            }
            return new TalkGroup()
            {
                id = id,
                requires = requires,
                requiresNot = requiresNot,
                tags = tags,

                archive = archive,
                sections = sections,
            };
        }
    }
}
