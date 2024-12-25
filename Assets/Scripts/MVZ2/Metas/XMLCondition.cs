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
            var required = node.GetAttributeNamespaceIDArray("required", defaultNsp);
            var requiredNot = node.GetAttributeNamespaceIDArray("requiredNot", defaultNsp);
            return new XMLCondition()
            {
                Required = required,
                RequiredNot = requiredNot
            };
        }
    }
}
