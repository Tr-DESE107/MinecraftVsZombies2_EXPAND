using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class AchievementMetaList
    {
        public AchievementMeta[] metas;
        public static AchievementMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var metas = new List<AchievementMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                switch (childNode.Name)
                {
                    case "achievement":
                        var meta = AchievementMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
                        metas.Add(meta);
                        break;
                }
            }
            return new AchievementMetaList()
            {
                metas = metas.ToArray(),
            };
        }
    }
}
