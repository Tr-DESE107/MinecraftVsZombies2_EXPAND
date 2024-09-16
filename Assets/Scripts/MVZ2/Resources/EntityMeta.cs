using System.Collections.Generic;
using System.Xml;
using PVZEngine.Level;

namespace MVZ2
{
    public class EntityMeta
    {
        public int type;
        public string id;
        public string name;
        public string tooltip;
        public int order;
        public static EntityMeta FromXmlNode(XmlNode node)
        {
            var type = EntityTypes.EFFECT;
            if (entityTypeDict.TryGetValue(node.Name, out var t))
            {
                type = t;
            }
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var tooltip = node.GetAttribute("tooltip");
            return new EntityMeta()
            {
                type = type,
                id = id,
                name = name,
                tooltip = tooltip
            };
        }
        private static readonly Dictionary<string, int> entityTypeDict = new Dictionary<string, int>()
        {
            { "contraption", EntityTypes.PLANT },
            { "enemy", EntityTypes.ENEMY },
        };
    }
}
