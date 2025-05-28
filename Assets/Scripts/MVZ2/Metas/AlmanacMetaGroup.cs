﻿using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class AlmanacMetaGroup
    {
        public NamespaceID id;
        public string name;
        public int order;
        public AlmanacMetaEntry[] entries;
        public static AlmanacMetaGroup FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var order = node.GetAttributeInt("order") ?? 0;
            var name = node.GetAttribute("name");
            var entries = new AlmanacMetaEntry[node.ChildNodes.Count];
            int entryIndex = 0;
            for (int j = 0; j < entries.Length; j++)
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
                        entries[j] = entry;
                        break;
                }
            }
            return new AlmanacMetaGroup()
            {
                id = id,
                order = order,
                name = name,
                entries = entries
            };
        }
    }
}
