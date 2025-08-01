using System;
using System.Xml;
using MVZ2.IO;
using MVZ2.Saves;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.Metas
{
    public class ArtifactMeta : IArtifactMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public string Tooltip { get; private set; }
        [Obsolete]
        public NamespaceID Unlock { get; private set; }
        public XMLConditionList UnlockConditions { get; private set; }
        public SpriteReference Sprite { get; private set; }
        public int Order { get; private set; }
        public static ArtifactMeta FromXmlNode(XmlNode node, string defaultNsp, int order)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var tooltip = node.GetAttribute("tooltip");
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            var conditions = XMLConditionList.FromXmlNode(node["unlock"], defaultNsp);
            return new ArtifactMeta()
            {
                ID = id,
                Name = name,
                Tooltip = tooltip,
                Unlock = unlock,
                UnlockConditions = conditions,
                Order = order,
                Sprite = sprite,
            };
        }
        public bool IsUnlocked(IGameSaveData save)
        {
            bool unlockValid = NamespaceID.IsValid(Unlock);
            bool conditionValid = UnlockConditions != null;
            if (unlockValid || conditionValid)
            {
                if (conditionValid)
                {
                    if (save.MeetsXMLConditions(UnlockConditions))
                        return true;
                }

                if (unlockValid)
                {
                    if (save.IsUnlocked(Unlock))
                        return true;
                }

                return false;
            }

            return true; // 没有解锁条件，默认解锁。
        }
    }
}
