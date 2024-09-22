using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Resources
{
    public class AlmanacMetaList
    {
        public Dictionary<string, AlmanacMetaEntry[]> entries;
        public static AlmanacMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var entries = new Dictionary<string, AlmanacMetaEntry[]>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var categoryNode = node.ChildNodes[i];
                var typeName = categoryNode.Name;
                var categoryEntries = new AlmanacMetaEntry[categoryNode.ChildNodes.Count];
                for (int j = 0; j < categoryEntries.Length; j++)
                {
                    categoryEntries[j] = AlmanacMetaEntry.FromXmlNode(categoryNode.ChildNodes[j], defaultNsp);
                }
                entries.Add(typeName, categoryEntries);
            }
            return new AlmanacMetaList()
            {
                entries = entries,
            };
        }
    }
}
