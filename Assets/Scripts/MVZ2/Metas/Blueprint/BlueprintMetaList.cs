using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class BlueprintMetaList
    {
        public BlueprintOptionMeta[] Options { get; private set; }
        public BlueprintErrorMeta[] Errors { get; private set; }
        public static BlueprintMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var options = new List<BlueprintOptionMeta>();
            var errors = new List<BlueprintErrorMeta>();
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "option")
                {
                    options.Add(BlueprintOptionMeta.FromXmlNode(child, defaultNsp));
                }
                else if (child.Name == "errors")
                {
                    LoadErrors(errors, child, defaultNsp);
                }
            }
            return new BlueprintMetaList()
            {
                Options = options.ToArray(),
                Errors = errors.ToArray(),
            };
        }
        private static void LoadErrors(List<BlueprintErrorMeta> errors, XmlNode node, string defaultNsp)
        {
            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "error")
                {
                    errors.Add(BlueprintErrorMeta.FromXmlNode(child, defaultNsp));
                }
            }
        }
    }
}
