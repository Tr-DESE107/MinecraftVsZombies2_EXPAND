﻿using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Metas
{
    public class AlmanacTagMeta
    {
        public string id;
        public string name;
        public string description;
        public int priority;
        public NamespaceID enumType;

        public SpriteReference iconSprite;

        public SpriteReference backgroundSprite;
        public Color backgroundColor;

        public SpriteReference markSprite;
        public static AlmanacTagMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var description = node.GetAttribute("description");
            var priority = node.GetAttributeInt("priority") ?? 0;
            var enumType = node.GetAttributeNamespaceID("enum", defaultNsp);

            var iconNode = node["icon"];
            SpriteReference iconSprite = null;
            if (iconNode != null)
            {
                iconSprite = iconNode.GetAttributeSpriteReference("sprite", defaultNsp);
            }

            var backgroundNode = node["background"];
            SpriteReference backgroundSprite = null;
            Color backgroundColor = Color.gray;
            if (backgroundNode != null)
            {
                backgroundSprite = backgroundNode.GetAttributeSpriteReference("sprite", defaultNsp);
                backgroundColor = backgroundNode.GetAttributeColor("color") ?? backgroundColor;
            }

            var markNode = node["mark"];
            SpriteReference markSprite = null;
            if (markNode != null)
            {
                markSprite = markNode.GetAttributeSpriteReference("sprite", defaultNsp);
            }

            return new AlmanacTagMeta()
            {
                id = id,
                name = name,
                description = description,
                priority = priority,
                enumType = enumType,

                iconSprite = iconSprite,

                backgroundSprite = backgroundSprite,
                backgroundColor = backgroundColor,

                markSprite = markSprite,
            };
        }
    }
}
