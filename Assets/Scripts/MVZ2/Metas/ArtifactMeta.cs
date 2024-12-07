using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using MVZ2Logic.Entities;
using PVZEngine;

namespace MVZ2.Metas
{
    public class ArtifactMeta : IArtifactMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Tooltip { get; private set; }
        public NamespaceID Unlock { get; private set; }
        public SpriteReference Sprite { get; private set; }
        public NamespaceID BuffID { get; private set; }
        public int Order { get; private set; }
        public static ArtifactMeta FromXmlNode(XmlNode node, string defaultNsp, int order)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var tooltip = node.GetAttribute("tooltip");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var buffId = node.GetAttributeNamespaceID("buffId", defaultNsp);
            return new ArtifactMeta()
            {
                ID = id,
                Name = name,
                Tooltip = tooltip,
                Unlock = unlock,
                Order = order,
                Sprite = sprite,
                BuffID = buffId,
            };
        }
    }
}
