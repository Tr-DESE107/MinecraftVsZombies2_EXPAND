using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2.Saves;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.Metas
{
    public class LoreTalkMetaList
    {
        public LoreTalkMeta[] Talks { get; set; }
        public static LoreTalkMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            if (node == null)
                return null;
            List<LoreTalkMeta> metas = new List<LoreTalkMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "talk")
                {
                    metas.Add(LoreTalkMeta.FromXmlNode(child, defaultNsp));
                }
            }
            return new LoreTalkMetaList()
            {
                Talks = metas.ToArray()
            };
        }
        public NamespaceID[] GetLoreTalks(IGlobalSaveData save)
        {
            List<NamespaceID> results = new List<NamespaceID>();
            foreach (var talk in Talks)
            {
                if (talk.Conditions != null && save.MeetsXMLConditions(talk.Conditions))
                {
                    results.Add(talk.ID);
                }
            }
            return results.ToArray();
        }
    }
    public class LoreTalkMeta
    {
        public NamespaceID ID { get; set; }
        public XMLConditionList Conditions { get; set; }
        public static LoreTalkMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var conditions = XMLConditionList.FromXmlNode(node["conditions"], defaultNsp);
            return new LoreTalkMeta()
            {
                ID = id,
                Conditions = conditions,
            };
        }
    }
}
