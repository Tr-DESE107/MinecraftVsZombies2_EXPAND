using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class BlueprintMetaIcon
    {
        public SpriteReference Mobile { get; private set; }
        public SpriteReference Sprite { get; private set; }
        public NamespaceID ModelID { get; private set; }
        public static BlueprintMetaIcon FromXmlNode(XmlNode node, string defaultNsp, NamespaceID blueprintID)
        {
            var mobile = node.GetAttributeSpriteReference("mobile", defaultNsp);
            if (!SpriteReference.IsValid(mobile))
            {
                var id = new NamespaceID(blueprintID.SpaceName, $"mobile_blueprint/{blueprintID.Path}");
                mobile = new SpriteReference(id);
            }
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            return new BlueprintMetaIcon()
            {
                Sprite = sprite,
                ModelID = model,
                Mobile = mobile,
            };
        }
    }
}
