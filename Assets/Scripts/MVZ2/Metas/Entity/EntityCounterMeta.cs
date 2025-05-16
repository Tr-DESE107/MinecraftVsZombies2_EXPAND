using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Entities;

namespace MVZ2.Metas
{
    public class EntityCounterMeta : IEntityCounterMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public static EntityCounterMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");

            return new EntityCounterMeta()
            {
                ID = id,
                Name = name,
            };
        }
    }
}
