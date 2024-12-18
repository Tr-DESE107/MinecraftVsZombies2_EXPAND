using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.Metas
{
    public class ProgressBarMeta
    {
        public string ID { get; private set; }
        public string Type { get; private set; }
        public Vector2 Size { get; private set; }

        public SpriteReference BackgroundSprite { get; private set; }

        public SpriteReference BarSprite { get; private set; }
        public bool FromLeft { get; private set; }

        public Vector4 Padding { get; private set; }

        public SpriteReference IconSprite { get; private set; }
        public static ProgressBarMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var type = node.GetAttribute("type");
            var width = node.GetAttributeInt("width") ?? 0;
            var height = node.GetAttributeInt("height") ?? 0;

            var backgroundNode = node["background"];
            SpriteReference background = null;
            if (backgroundNode != null)
            {
                background = backgroundNode.GetAttributeSpriteReference("sprite", defaultNsp);
            }

            var barNode = node["bar"];
            SpriteReference barSprite = null;
            bool fromLeft = false;
            if (barNode != null)
            {
                barSprite = barNode.GetAttributeSpriteReference("sprite", defaultNsp);
                fromLeft = barNode.GetAttributeBool("fromLeft") ?? fromLeft;
            }

            var paddingNode = node["padding"];
            Vector4 padding = Vector4.zero;
            if (paddingNode != null)
            {
                padding.x = paddingNode.GetAttributeFloat("left") ?? 0;
                padding.y = paddingNode.GetAttributeFloat("bottom") ?? 0;
                padding.z = paddingNode.GetAttributeFloat("right") ?? 0;
                padding.w = paddingNode.GetAttributeFloat("top") ?? 0;
            }

            var iconNode = node["icon"];
            SpriteReference iconSprite = null;
            if (iconNode != null)
            {
                iconSprite = iconNode.GetAttributeSpriteReference("sprite", defaultNsp);
            }
            return new ProgressBarMeta()
            {
                ID = id,
                Type = type,
                Size = new Vector2(width, height),

                BackgroundSprite = background,

                BarSprite = barSprite,
                FromLeft = fromLeft,

                Padding = padding,

                IconSprite = iconSprite,
            };
        }
    }
}
