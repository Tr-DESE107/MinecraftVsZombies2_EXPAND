using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class MinigameMetaList
    {
        public MinigameMeta[] metas;
        public static MinigameMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new MinigameMeta[node.ChildNodes.Count];
            ConcurrentDictionary<string, int> indexes = new ConcurrentDictionary<string, int>();
            for (int i = 0; i < resources.Length; i++)
            { 
                var childNode = node.ChildNodes[i];
                var key = childNode.Name;
                var index = indexes.GetOrAdd(key, 0);
                indexes[key]++;
                resources[i] = MinigameMeta.FromXmlNode(childNode, defaultNsp, index);
            }
            return new MinigameMetaList()
            {
                metas = resources,
            };
        }
    }
}
