using System.Xml;

namespace MVZ2.Metas
{
    public class ArmorMetaList
    {
        public ArmorSlotMeta[] slots;
        public ArmorMeta[] metas;
        public static ArmorMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var slotsNode = node["slots"];
            var slots = new ArmorSlotMeta[slotsNode.ChildNodes.Count];
            for (int i = 0; i < slots.Length; i++)
            {
                var meta = ArmorSlotMeta.FromXmlNode(slotsNode.ChildNodes[i], defaultNsp);
                slots[i] = meta;
            }

            var entriesNode = node["entries"];
            var resources = new ArmorMeta[entriesNode.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = ArmorMeta.FromXmlNode(entriesNode.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new ArmorMetaList()
            {
                slots = slots,
                metas = resources,
            };
        }
    }
}
