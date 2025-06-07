using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class AlmanacCategory
    {
        public string name;
        public AlmanacMetaGroup[] groups;
        public AlmanacMetaEntry[] entries;
        public static AlmanacCategory FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.Name;
            var groups = new List<AlmanacMetaGroup>();
            var entries = new List<AlmanacMetaEntry>();
            int entryIndex = 0;
            for (int j = 0; j < node.ChildNodes.Count; j++)
            {
                var childNode = node.ChildNodes[j];
                switch (childNode.Name)
                {
                    case "entry":
                        var entry = AlmanacMetaEntry.FromXmlNode(childNode, defaultNsp);
                        if (!entry.hidden)
                        {
                            entry.index = entryIndex;
                            entryIndex++;
                        }
                        entries.Add(entry);
                        break;
                    case "group":
                        var group = AlmanacMetaGroup.FromXmlNode(childNode, defaultNsp);
                        groups.Add(group);
                        break;
                }
            }
            return new AlmanacCategory()
            {
                name = name,
                groups = groups.ToArray(),
                entries = entries.ToArray()
            };
        }
    }
}
