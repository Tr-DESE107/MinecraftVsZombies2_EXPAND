using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using PVZEngine;

namespace MVZ2.Metas
{
    public class XMLCondition
    {
        public NamespaceID[] Required { get; private set; }
        public NamespaceID[] RequiredNot { get; private set; }
        public static XMLCondition FromXmlNode(XmlNode node, string defaultNsp)
        {
            List<NamespaceID> required = new List<NamespaceID>();
            var requiredStr = node.GetAttribute("required");
            if (!string.IsNullOrEmpty(requiredStr))
            {
                var strings = requiredStr.Split(";");
                foreach (var s in strings)
                {
                    if (NamespaceID.TryParse(s, defaultNsp, out var parsed))
                    {
                        required.Add(parsed);
                    }
                }
            }
            List<NamespaceID> requiredNot = new List<NamespaceID>();
            var requiredNotStr = node.GetAttribute("requiredNot");
            if (!string.IsNullOrEmpty(requiredNotStr))
            {
                var strings = requiredNotStr.Split(";");
                foreach (var s in strings)
                {
                    if (NamespaceID.TryParse(s, defaultNsp, out var parsed))
                    {
                        requiredNot.Add(parsed);
                    }
                }
            }
            return new XMLCondition()
            {
                Required = required.ToArray(),
                RequiredNot = requiredNot.ToArray()
            };
        }
    }
}
