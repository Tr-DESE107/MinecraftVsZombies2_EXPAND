using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Entities;
using PVZEngine;

namespace MVZ2.Metas
{
    public class ArmorSlotMeta : IArmorSlotMeta
    {
        public string Name { get; private set; }
        public ArmorSlotAnchorMeta[] Anchors { get; private set; }
        IArmorSlotAnchorMeta[] IArmorSlotMeta.Anchors => Anchors;
        public static ArmorSlotMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var name = node.GetAttribute("name");

            var anchors = new List<ArmorSlotAnchorMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                var anchorMeta = ArmorSlotAnchorMeta.FromXmlNode(child, defaultNsp);
                anchors.Add(anchorMeta);
            }
            return new ArmorSlotMeta()
            {
                Name = name,
                Anchors = anchors.ToArray()
            };
        }
    }
    public class ArmorSlotAnchorMeta : IArmorSlotAnchorMeta
    {
        public string Anchor { get; private set; }
        public NamespaceID Tag { get; private set; }
        public static ArmorSlotAnchorMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var anchor = node.GetAttribute("anchor");
            var tag = node.GetAttributeNamespaceID("tag", defaultNsp);
            return new ArmorSlotAnchorMeta()
            {
                Anchor = anchor,
                Tag = tag
            };
        }
    }
}
