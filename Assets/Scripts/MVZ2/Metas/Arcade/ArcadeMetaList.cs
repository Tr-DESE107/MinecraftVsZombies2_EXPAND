﻿using System.Collections.Concurrent;
using System.Xml;

namespace MVZ2.Metas
{
    public class ArcadeMetaList
    {
        public ArcadeMeta[] metas;
        public static ArcadeMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new ArcadeMeta[node.ChildNodes.Count];
            ConcurrentDictionary<string, int> indexes = new ConcurrentDictionary<string, int>();
            for (int i = 0; i < resources.Length; i++)
            {
                var childNode = node.ChildNodes[i];
                var key = childNode.Name;
                var index = indexes.GetOrAdd(key, 0);
                indexes[key]++;
                resources[i] = ArcadeMeta.FromXmlNode(childNode, defaultNsp, index);
            }
            return new ArcadeMetaList()
            {
                metas = resources,
            };
        }
    }
}
