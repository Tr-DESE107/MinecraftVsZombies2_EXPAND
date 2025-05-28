﻿using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class ArcadeMeta
    {
        public string ID { get; private set; }
        public string Type { get; private set; }
        public int Index { get; private set; }
        public NamespaceID AreaID { get; private set; }
        public NamespaceID StageID { get; private set; }
        public SpriteReference Icon { get; private set; }
        public NamespaceID[] HiddenUntil { get; private set; }

        public static ArcadeMeta FromXmlNode(XmlNode node, string defaultNsp, int index)
        {
            var id = node.GetAttribute("id");
            var type = node.Name;
            var area = node.GetAttributeNamespaceID("area", defaultNsp);
            var stage = node.GetAttributeNamespaceID("stage", defaultNsp);
            var icon = node.GetAttributeSpriteReference("icon", defaultNsp);
            var hiddenUntil = node.GetAttributeNamespaceIDArray("hiddenUntil", defaultNsp);

            return new ArcadeMeta()
            {
                ID = id,
                Type = type,
                AreaID = area,
                StageID = stage,
                Icon = icon,
                HiddenUntil = hiddenUntil,
                Index = index
            };
        }
    }
}
