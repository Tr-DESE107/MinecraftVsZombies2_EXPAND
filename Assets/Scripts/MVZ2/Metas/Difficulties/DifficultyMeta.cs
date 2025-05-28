﻿using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;

namespace MVZ2.Metas
{
    public class DifficultyMeta : IDifficultyMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int Value { get; private set; }
        public NamespaceID BuffID { get; private set; }
        public NamespaceID IZombieBuffID { get; private set; }

        public int CartConvertMoney { get; private set; }
        public int ClearMoney { get; private set; }
        public int RerunClearMoney { get; private set; }
        public int PuzzleMoney { get; private set; }

        public SpriteReference MapButtonBorderBack { get; private set; }
        public SpriteReference MapButtonBorderBottom { get; private set; }
        public SpriteReference MapButtonBorderOverlay { get; private set; }
        public SpriteReference ArcadeIcon { get; private set; }


        public static DifficultyMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var value = node.GetAttributeInt("value") ?? 0;

            var buffID = node["buff"]?.GetAttributeNamespaceID("id", defaultNsp);
            var IZBuffID = node["buff"]?.GetAttributeNamespaceID("iZombie", defaultNsp);

            int cartMoney = 50;
            int clearMoney = 0;
            int rerunMoney = 250;
            int puzzleMoney = 1000;
            var clearNode = node["clear"];
            if (clearNode != null)
            {
                cartMoney = clearNode.GetAttributeInt("cartMoney") ?? 50;
                clearMoney = clearNode.GetAttributeInt("clearMoney") ?? 0;
                rerunMoney = clearNode.GetAttributeInt("rerunMoney") ?? 250;
                puzzleMoney = clearNode.GetAttributeInt("puzzleMoney") ?? 1000;
            }

            SpriteReference backSprite = null;
            SpriteReference bottomSprite = null;
            SpriteReference overlaySprite = null;
            var buttonNode = node["button"];
            if (buttonNode != null)
            {
                backSprite = buttonNode.GetAttributeSpriteReference("back", defaultNsp);
                bottomSprite = buttonNode.GetAttributeSpriteReference("bottom", defaultNsp);
                overlaySprite = buttonNode.GetAttributeSpriteReference("overlay", defaultNsp);
            }

            SpriteReference arcadeIcon = null;
            var iconNode = node["icon"];
            if (iconNode != null)
            {
                arcadeIcon = iconNode.GetAttributeSpriteReference("arcade", defaultNsp);
            }

            return new DifficultyMeta()
            {
                ID = id,
                Name = name,
                Value = value,
                BuffID = buffID,
                IZombieBuffID = IZBuffID,

                CartConvertMoney = cartMoney,
                ClearMoney = clearMoney,
                RerunClearMoney = rerunMoney,
                PuzzleMoney = puzzleMoney,

                MapButtonBorderBack = backSprite,
                MapButtonBorderBottom = bottomSprite,
                MapButtonBorderOverlay = overlaySprite,
                ArcadeIcon = arcadeIcon
            };
        }
    }
}
