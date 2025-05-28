﻿using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;

namespace MVZ2.Metas
{
    public class CreditsCategoryMeta
    {
        public string Name { get; private set; }
        public string[] Entries { get; private set; }
        public static CreditsCategoryMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");

            List<string> entries = new List<string>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "entry")
                {
                    entries.Add(child.InnerText);
                }
            }
            return new CreditsCategoryMeta()
            {
                Name = name,
                Entries = entries.ToArray(),
            };
        }
    }
}
