using System.Collections.Generic;
using System.IO;
using System.Xml;
using MVZ2.Resources;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public static class XMLHelper
    {
        public static XmlAttribute CreateAttribute(this XmlNode node, string name, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            var attr = node.OwnerDocument.CreateAttribute(name);
            attr.Value = value;
            node.Attributes.Append(attr);
            return attr;
        }
        public static XmlDocument ReadXmlDocument(this string str)
        {
            using var memory = new MemoryStream();
            using var textWriter = new StreamWriter(memory);
            textWriter.Write(str);
            memory.Seek(0, SeekOrigin.Begin);
            return memory.ReadXmlDocument();
        }
        public static XmlDocument ReadXmlDocument(this Stream stream)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            using var xmlReader = XmlReader.Create(stream, settings);
            var document = new XmlDocument();
            document.Load(xmlReader);
            return document;
        }
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
            if (!ParseHelper.TryParseInt(attr.Value, out var value))
                return null;
            return value;
        }
        public static float? GetAttributeFloat(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!ParseHelper.TryParseFloat(attr.Value, out var value))
                return null;
            return value;
        }
        public static Color? GetAttributeColor(this XmlNode node, string name)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!ColorUtility.TryParseHtmlString(attr.Value, out var value))
                return null;
            return value;
        }
        public static NamespaceID GetAttributeNamespaceID(this XmlNode node, string name, string defaultNsp)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!NamespaceID.TryParse(attr.Value, defaultNsp, out var value))
                return null;
            return value;
        }
        public static SpriteReference GetAttributeSpriteReference(this XmlNode node, string name, string defaultNsp)
        {
            var attr = node.Attributes[name];
            if (attr == null)
                return null;
            if (!SpriteReference.TryParse(attr.Value, defaultNsp, out var value))
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
        public static Dictionary<string, object> ToPropertyDictionary(this XmlNode node, string defaultNsp)
        {
            var properties = new Dictionary<string, object>();
            if (node != null)
            {
                for (int i = 0; i < node.ChildNodes.Count; i++)
                {
                    var propNode = node.ChildNodes[i];
                    var propKey = propNode.GetAttribute("name");
                    if (propNode.TryToProperty(defaultNsp, out var propValue))
                    {
                        properties.Add(propKey, propValue);
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot create property \"{propKey}\" of type \"{propNode.Name}\" from xml node.");
                    }
                }
            }
            return properties;
        }
        public static bool TryToProperty(this XmlNode node, string defaultNsp, out object propValue)
        {
            propValue = null;
            var propName = node.Name;
            switch (propName)
            {
                case "bool":
                    {
                        var value = node.GetAttributeBool("value");
                        bool valid = value.HasValue;
                        if (valid)
                            propValue = value.Value;
                        return valid;
                    }
                case "int":
                case "integer":
                    {
                        var value = node.GetAttributeInt("value");
                        bool valid = value.HasValue;
                        if (valid)
                            propValue = value.Value;
                        return valid;
                    }
                case "float":
                    {
                        var value = node.GetAttributeFloat("value");
                        bool valid = value.HasValue;
                        if (valid)
                            propValue = value.Value;
                        return valid;
                    }
                case "vector2":
                    {
                        var x = node.GetAttributeFloat("x");
                        var y = node.GetAttributeFloat("y");
                        bool valid = x.HasValue && y.HasValue;
                        if (valid)
                            propValue = new Vector2(x.Value, y.Value);
                        return valid;
                    }
                case "vector3":
                    {
                        var x = node.GetAttributeFloat("x");
                        var y = node.GetAttributeFloat("y");
                        var z = node.GetAttributeFloat("z");
                        bool valid = x.HasValue && y.HasValue && z.HasValue;
                        if (valid)
                            propValue = new Vector3(x.Value, y.Value, z.Value);
                        return valid;
                    }
                case "color":
                    {
                        var value = node.GetAttributeColor("value");
                        bool valid = value.HasValue;
                        if (valid)
                            propValue = value.Value;
                        return valid;
                    }
                case "id":
                    {
                        if (node.GetAttributeBool("null") ?? false)
                        {
                            propValue = null;
                            return true;
                        }
                        var value = node.GetAttributeNamespaceID("value", defaultNsp);
                        bool valid = value != null;
                        if (valid)
                            propValue = value;
                        return valid;
                    }
                case "sprite":
                    {
                        if (node.GetAttributeBool("null") ?? false)
                        {
                            propValue = null;
                            return true;
                        }
                        var value = node.GetAttributeSpriteReference("value", defaultNsp);
                        bool valid = value != null;
                        if (valid)
                            propValue = value;
                        return valid;
                    }
            }
            return false;
        }
    }
}
