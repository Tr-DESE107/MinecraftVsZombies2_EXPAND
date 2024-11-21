using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Talk
{
    public class TalkGroup
    {
        public string name;
        public NamespaceID requires;
        public NamespaceID requiresNot;
        public SpriteReference archiveBackground;
        public NamespaceID music;
        public List<NamespaceID> tags;
        public List<TalkSection> sections;
        public XmlNode ToXmlNode(XmlDocument document)
        {
            XmlNode node = document.CreateElement("group");
            node.CreateAttribute("name", name);
            node.CreateAttribute("requires", requires?.ToString());
            node.CreateAttribute("requiresNot", requiresNot?.ToString());
            node.CreateAttribute("background", archiveBackground?.ToString());
            node.CreateAttribute("music", music?.ToString());
            node.CreateAttribute("tags", tags != null ? string.Join(";", tags.Select(t => t.ToString())) : null);
            foreach (var section in sections)
            {
                var child = section.ToXmlNode(document);
                node.AppendChild(child);
            }
            return node;
        }
        public static TalkGroup FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var requires = node.GetAttributeNamespaceID("requires", defaultNsp);
            var requiresNot = node.GetAttributeNamespaceID("requiresNot", defaultNsp);
            var background = node.GetAttributeSpriteReference("background", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            var tags = node.GetAttribute("tags")?.Split(';')?.Select(t => NamespaceID.Parse(t, defaultNsp))?.ToList();

            var children = node.ChildNodes;
            var sections = new List<TalkSection>();
            for (int i = 0; i < children.Count; i++)
            {
                var child = children[i];
                sections.Add(TalkSection.FromXmlNode(child, defaultNsp));
            }
            return new TalkGroup()
            {
                name = name,
                requires = requires,
                requiresNot = requiresNot,
                archiveBackground = background,
                music = music,
                tags = tags,
                sections = sections,
            };
        }
    }
}
