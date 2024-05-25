using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

namespace MVZ2
{
    public static class XMLHelper
    {
        public static string GetAttribute(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            return attr.Value;
        }
        public static bool? GetAttributeBool(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!bool.TryParse(attr.Value, out var value))
                return null;
            return value;
        }
        public static int? GetAttributeInt(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!int.TryParse(attr.Value, out var value))
                return null;
            return value;
        }
        public static float? GetAttributeFloat(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!float.TryParse(attr.Value, out var value))
                return null;
            return value;
        }
        public static Gradient ToGradient(this XmlNode node)
        {
            if (node == null)
                return null;
            var blend = node.GetAttributeBool("blend") ?? false;

            GradientColorKey[] colorKeys;
            var colorKeysNode = node["colorKeys"];
            if (colorKeysNode?.ChildNodes == null || colorKeysNode.ChildNodes.Count <= 0)
            {
                colorKeys = new GradientColorKey[]
                {
                    new GradientColorKey(Color.magenta, 0),
                    new GradientColorKey(Color.black, 0.5f),
                };
            }
            else
            {
                colorKeys = new GradientColorKey[colorKeysNode.ChildNodes.Count];
                for (int i = 0; i < colorKeys.Length; i++)
                {
                    colorKeys[i] = colorKeysNode.ChildNodes[i].ToGradientColorKey();
                }
            }

            GradientAlphaKey[] alphaKeys;
            var alphaKeysNode = node["alphaKeys"];
            if (alphaKeysNode?.ChildNodes == null || alphaKeysNode.ChildNodes.Count <= 0)
            {
                alphaKeys = new GradientAlphaKey[]
                {
                    new GradientAlphaKey(1, 0)
                };
            }
            else
            {
                alphaKeys = new GradientAlphaKey[alphaKeysNode.ChildNodes.Count];
                for (int i = 0; i < alphaKeys.Length; i++)
                {
                    alphaKeys[i] = alphaKeysNode.ChildNodes[i].ToGradientAlphaKey();
                }
            }

            return new Gradient()
            {
                mode = blend ? GradientMode.Blend : GradientMode.Fixed,
                colorKeys = colorKeys,
                alphaKeys = alphaKeys
            };
        }
        public static GradientColorKey ToGradientColorKey(this XmlNode node)
        {
            var col = ColorUtility.TryParseHtmlString(node.GetAttribute("hex"), out var color) ? color : Color.magenta;
            var time = node.GetAttributeFloat("time") ?? 0;
            return new GradientColorKey(col, time);
        }
        public static GradientAlphaKey ToGradientAlphaKey(this XmlNode node)
        {
            var alpha = node.GetAttributeFloat("alpha") ?? 1;
            var time = node.GetAttributeFloat("time") ?? 0;
            return new GradientAlphaKey(alpha, time);
        }
    }
}
