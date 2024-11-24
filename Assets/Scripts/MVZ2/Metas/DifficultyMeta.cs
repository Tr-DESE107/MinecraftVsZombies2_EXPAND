using MVZ2.IO;
using System.Xml;

namespace MVZ2.Metas
{
    public class DifficultyMeta
    {
        public string id;
        public string name;
        public static DifficultyMeta FromXmlNode(XmlNode node)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            return new DifficultyMeta()
            {
                id = id,
                name = name
            };
        }
    }
}
