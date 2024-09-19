using System.Xml;
using UnityEngine;

namespace MVZ2
{
    public class NoteMeta
    {
        public string id;
        public SpriteReference sprite;
        public SpriteReference background;
        public bool canFlip;
        public SpriteReference flipSprite;
        public static NoteMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var background = node.GetAttributeSpriteReference("background", defaultNsp);
            var canFlip = node.GetAttributeBool("canFlip") ?? false;
            var flipSprite = node.GetAttributeSpriteReference("flipSprite", defaultNsp);
            return new NoteMeta()
            {
                id = id,
                sprite = sprite,
                background = background,
                canFlip = canFlip,
                flipSprite = flipSprite
            };
        }
    }
}
