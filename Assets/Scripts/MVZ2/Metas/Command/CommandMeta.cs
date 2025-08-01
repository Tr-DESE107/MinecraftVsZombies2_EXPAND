using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;

namespace MVZ2.Metas
{
    public class CommandMeta
    {
        public string ID { get; private set; }
        public CommandMetaVariant[] Variants { get; private set; }
        public static CommandMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var variants = new List<CommandMetaVariant>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                if (childNode.Name == "variant")
                {
                    variants.Add(CommandMetaVariant.FromXmlNode(childNode, defaultNsp));
                }
            }
            return new CommandMeta()
            {
                ID = id,
                Variants = variants.ToArray(),
            };
        }
    }
    public class CommandMetaVariant
    {
        public string Subname { get; private set; }
        public string Description { get; private set; }
        public CommandMetaParam[] Parameters { get; private set; }
        public static CommandMetaVariant FromXmlNode(XmlNode node, string defaultNsp)
        {
            var subname = node.GetAttribute("subname");

            var description = node["description"]?.InnerText ?? string.Empty;

            var paramList = new List<CommandMetaParam>();
            var paramsNode = node["params"];
            for (int i = 0; i < paramsNode.ChildNodes.Count; i++)
            {
                var childNode = paramsNode.ChildNodes[i];
                if (childNode.Name == "param")
                {
                    paramList.Add(CommandMetaParam.FromXmlNode(childNode, defaultNsp));
                }
            }
            return new CommandMetaVariant()
            {
                Subname = subname,
                Description = description,
                Parameters = paramList.ToArray()
            };
        }
    }
    public class CommandMetaParam
    {
        public string Name { get; private set; }
        public string Type { get; private set; }
        public string IDType { get; private set; }
        public bool Optional { get; private set; }
        public string Description { get; private set; }
        public static CommandMetaParam FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type");
            var idType = node.GetAttribute("idType");
            var optional = node.GetAttributeBool("optional") ?? false;
            var description = node.InnerText;
            return new CommandMetaParam()
            {
                Name = name,
                Type = type,
                IDType = idType,
                Optional = optional,
                Description = description
            };
        }
    }
}
