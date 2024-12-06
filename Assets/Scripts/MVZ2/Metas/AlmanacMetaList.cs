using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MVZ2.Metas
{
    public class AlmanacMetaList
    {
        public List<AlmanacCategory> categories;
        public AlmanacCategory GetCategory(string name)
        {
            return categories.FirstOrDefault(c => c.name == name);
        }
        public bool TryGetCategory(string name, out AlmanacCategory category)
        {
            category = GetCategory(name);
            return category != null;
        }
        public static AlmanacMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var categories = new List<AlmanacCategory>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var categoryNode = node.ChildNodes[i];
                var category = AlmanacCategory.FromXmlNode(categoryNode, defaultNsp);
                categories.Add(category);
            }
            return new AlmanacMetaList()
            {
                categories = categories,
            };
        }
    }
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
                        entry.index = entryIndex;
                        entries.Add(entry);
                        entryIndex++;
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
