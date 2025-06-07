﻿using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using UnityEngine;

namespace MVZ2.Metas
{
    public class AlmanacTagEnumValueMeta
    {
        public string name;
        public string description;

        public SpriteReference iconSprite;
        public Color backgroundColor;

        public object value;
        public static AlmanacTagEnumValueMeta FromXmlNode(XmlNode node, string type, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var description = node.GetAttribute("description");

            SpriteReference iconSprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            Color backgroundColor = node.GetAttributeColor("backgroundColor") ?? Color.gray;

            object value = null;
            if (node.TryGetAttributeStruct("value", type, out var propValue))
            {
                value = propValue;
            }
            else if (node.TryGetAttributeNullable("value", "null", type, defaultNsp, out propValue))
            {
                value = propValue;
            }

            return new AlmanacTagEnumValueMeta()
            {
                name = name,
                description = description,

                iconSprite = iconSprite,
                backgroundColor = backgroundColor,

                value = value
            };
        }
    }
}
