﻿using System.Xml;

namespace MVZ2.Metas
{
    public class DifficultyMetaList
    {
        public DifficultyMeta[] metas;
        public static DifficultyMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new DifficultyMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = DifficultyMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new DifficultyMetaList()
            {
                metas = resources,
            };
        }
    }
}
