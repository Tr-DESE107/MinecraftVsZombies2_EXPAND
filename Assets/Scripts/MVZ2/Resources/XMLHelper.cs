using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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
    }
}
