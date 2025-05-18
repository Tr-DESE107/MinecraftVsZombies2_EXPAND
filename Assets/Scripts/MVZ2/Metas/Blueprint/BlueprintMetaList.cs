using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class BlueprintMetaList
    {
        public BlueprintOptionMeta[] Options { get; private set; }
        public static BlueprintMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var options = new List<BlueprintOptionMeta>();
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "option")
                {
                    options.Add(BlueprintOptionMeta.FromXmlNode(child, defaultNsp));
                }
            }
            return new BlueprintMetaList()
            {
                Options = options.ToArray()
            };
        }
    }
}
