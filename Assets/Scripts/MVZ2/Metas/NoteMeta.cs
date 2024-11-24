using System.Xml;
using PVZEngine;
using MVZ2.IO;
using MVZ2Logic;

namespace MVZ2.Metas
{
    public class NoteMeta
    {
        public string id;
        public SpriteReference sprite;
        public SpriteReference background;
        public NamespaceID startTalk;
        public bool canFlip;
        public SpriteReference flipSprite;
        public static NoteMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var background = node.GetAttributeSpriteReference("background", defaultNsp);
            var startTalk = node.GetAttributeNamespaceID("startTalk", defaultNsp);
            var canFlip = node.GetAttributeBool("canFlip") ?? false;
            var flipSprite = node.GetAttributeSpriteReference("flipSprite", defaultNsp);
            return new NoteMeta()
            {
                id = id,
                sprite = sprite,
                background = background,
                startTalk = startTalk,
                canFlip = canFlip,
                flipSprite = flipSprite
            };
        }
    }
}
