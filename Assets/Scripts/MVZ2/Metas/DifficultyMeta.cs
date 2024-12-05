using MVZ2.IO;
using System.Xml;

namespace MVZ2.Metas
{
    public class DifficultyMeta
    {
        public string id;
        public string name;
        public int value;
        public static DifficultyMeta FromXmlNode(XmlNode node)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var value = node.GetAttributeInt("value") ?? 0;
            return new DifficultyMeta()
            {
                id = id,
                name = name,
                value = value
            };
        }
    }
}
