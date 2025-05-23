﻿using System.Xml;
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
        public SpriteReference ForegroundSprite { get; private set; }
        public bool FromLeft { get; private set; }
        public ProgressBarMode BarMode { get; private set; }

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

            var foregroundNode = node["foreground"];
            SpriteReference foreground = null;
            if (foregroundNode != null)
            {
                foreground = foregroundNode.GetAttributeSpriteReference("sprite", defaultNsp);
            }

            var barNode = node["bar"];
            SpriteReference barSprite = null;
            bool fromLeft = false;
            ProgressBarMode barMode = ProgressBarMode.Sliced;
            if (barNode != null)
            {
                barSprite = barNode.GetAttributeSpriteReference("sprite", defaultNsp);
                fromLeft = barNode.GetAttributeBool("fromLeft") ?? fromLeft;
                barMode = ParseBarMode(barNode.GetAttribute("mode"));
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
                ForegroundSprite = foreground,

                BarSprite = barSprite,
                FromLeft = fromLeft,
                BarMode = barMode,

                Padding = padding,

                IconSprite = iconSprite,
            };
        }
        public static ProgressBarMode ParseBarMode(string str)
        {
            if (str == "filled")
                return ProgressBarMode.Filled;
            return ProgressBarMode.Sliced;
        }
    }
    public enum ProgressBarMode
    {
        Sliced = 0,
        Filled = 1
    }
}
