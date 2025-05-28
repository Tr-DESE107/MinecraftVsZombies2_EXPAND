﻿using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Entities;

namespace MVZ2.Metas
{
    public class ArmorSlotMeta : IArmorSlotMeta
    {
        public string Name { get; private set; }
        public string Anchor { get; private set; }
        public static ArmorSlotMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");
            var anchor = node.GetAttribute("anchor");
            return new ArmorSlotMeta()
            {
                Name = name,
                Anchor = anchor
            };
        }
    }
}
