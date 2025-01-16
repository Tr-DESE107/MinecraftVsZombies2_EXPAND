using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using MVZ2.IO;
using MVZ2.Saves;
using MVZ2Logic;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.Metas
{
    public class AlmanacMetaEntry
    {
        public NamespaceID id;
        public string name;
        public NamespaceID unlock;
        public SpriteReference sprite;
        public NamespaceID model;
        public string header;
        public string properties;
        public AlmanacMetaFlavor[] flavors;
        public int index = -1;
        public static AlmanacMetaEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var name = node.GetAttribute("name");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var headerNode = node["header"];
            var propertiesNode = node["properties"];
            var header = headerNode != null ? ConcatNodeParagraphs(headerNode) : string.Empty;
            var properties = propertiesNode != null ? ConcatNodeParagraphs(propertiesNode) : string.Empty;

            AlmanacMetaFlavor[] flavors;
            var flavorsNode = node["flavors"];
            var flavorNode = node["flavor"];
            if (flavorsNode != null)
            {
                var list = new List<AlmanacMetaFlavor>();
                for (int i = 0; i < flavorsNode.ChildNodes.Count; i++)
                {
                    var child = flavorsNode.ChildNodes[i];
                    if (child.Name == "flavor")
                    {
                        list.Add(AlmanacMetaFlavor.FromXmlNode(child, defaultNsp));
                    }
                }
                flavors = list.ToArray();
            }
            else if (flavorNode != null)
            {
                flavors = new AlmanacMetaFlavor[]
                {
                    AlmanacMetaFlavor.FromXmlNode(flavorNode, defaultNsp)
                };
            }
            else
            {
                flavors = Array.Empty<AlmanacMetaFlavor>();
            }
            return new AlmanacMetaEntry()
            {
                id = id,
                name = name,
                unlock = unlock,
                sprite = sprite,
                model = model,
                header = header,
                properties = properties,
                flavors = flavors,
            };
        }
        public bool IsEmpty()
        {
            return !NamespaceID.IsValid(id);
        }
        public string[] GetValidFlavors(IGameSaveData save)
        {
            return flavors.Where(f => f.conditions == null || save.MeetsXMLConditions(f.conditions)).Select(f => f.text).ToArray();
        }
        public string[] GetAllFlavors()
        {
            return flavors.Select(f => f.text).ToArray();
        }
        public static string ConcatNodeParagraphs(XmlNode node)
        {
            var lineNodes = node.ChildNodes;
            var sb = new StringBuilder();
            bool first = true;
            for (int i = 0; i < lineNodes.Count; i++)
            {
                var lineNode = lineNodes[i];
                if (lineNode.Name == "p")
                {
                    if (!first)
                    {
                        sb.Append("\n");
                    }
                    first = false;
                    sb.Append(lineNodes[i].InnerText);
                }
            }
            return sb.ToString();
        }
    }

    public class AlmanacMetaFlavor
    {
        public XMLConditionList conditions = new XMLConditionList();
        public string text;
        public static AlmanacMetaFlavor FromXmlNode(XmlNode node, string defaultNsp)
        {
            XMLConditionList conditions = null;
            var conditionsNode = node["conditions"];
            if (conditionsNode != null)
            {
                conditions = XMLConditionList.FromXmlNode(conditionsNode, defaultNsp);
            }
            var flavor = AlmanacMetaEntry.ConcatNodeParagraphs(node);
            return new AlmanacMetaFlavor()
            {
                conditions = conditions,
                text = flavor
            };
        }
    }
}
