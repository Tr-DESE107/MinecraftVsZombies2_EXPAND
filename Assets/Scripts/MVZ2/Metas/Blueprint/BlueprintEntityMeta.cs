using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class BlueprintEntityMeta
    {
        public string ID { get; private set; }
        public int Cost { get; private set; }
        public NamespaceID RechargeID { get; private set; }
        public string Name { get; private set; }
        public string Tooltip { get; private set; }
        public NamespaceID EntityID { get; private set; }
        public int Variant { get; private set; }
        public BlueprintMetaIcon Icon { get; private set; }
        public static BlueprintEntityMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var cost = node.GetAttributeInt("cost") ?? 0;
            var recharge = node.GetAttributeNamespaceID("recharge", defaultNsp);
            var name = node.GetAttribute("name");
            var tooltip = node.GetAttribute("tooltip");
            var entityID = node.GetAttributeNamespaceID("entity", defaultNsp);
            var variant = node.GetAttributeInt("variant") ?? 0;

            BlueprintMetaIcon icon = null;
            var iconNode = node["icon"];
            if (iconNode != null)
            {
                icon = BlueprintMetaIcon.FromXmlNode(iconNode, defaultNsp);
            }
            return new BlueprintEntityMeta()
            {
                ID = id,
                Cost = cost,
                RechargeID = recharge,
                Name = name,
                Tooltip = tooltip,
                EntityID = entityID,
                Variant = variant,

                Icon = icon,
            };
        }
        public SpriteReference GetIcon()
        {
            return Icon?.Sprite;
        }
        public SpriteReference GetMobileIcon()
        {
            return Icon?.Mobile;
        }
        public NamespaceID GetModelID()
        {
            return Icon?.ModelID;
        }

        public int GetCost() => Cost;

        public NamespaceID GetEntityID() => EntityID;

        public NamespaceID GetRechargeID() => RechargeID;

        public bool IsTriggerActive() => false;
        public bool CanInstantTrigger() => false;
        public bool CanInstantEvoke() => false;
        public bool IsUpgradeBlueprint() => false;
        public int GetVariant() => Variant;
    }
}
