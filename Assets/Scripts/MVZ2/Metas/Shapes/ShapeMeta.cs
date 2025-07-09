using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Entities;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Metas
{
    public class ShapeMeta : IShapeMeta
    {
        public string ID { get; private set; }
        public ShapeArmorMeta Armors { get; private set; }
        public Vector3 GetArmorPosition(NamespaceID slotID, NamespaceID armorID)
        {
            if (Armors == null)
                return Vector3.zero;
            return Armors.GetArmorPosition(slotID, armorID);
        }
        public Vector3 GetArmorScale(NamespaceID slotID, NamespaceID armorID)
        {
            if (Armors == null)
                return Vector3.one;
            return Armors.GetArmorScale(slotID, armorID);
        }
        public Vector3 GetArmorModelOffset(NamespaceID slotID, NamespaceID armorID)
        {
            if (Armors == null)
                return Vector3.zero;
            return Armors.GetArmorModelOffset(slotID, armorID);
        }
        public string GetArmorModelAnchor(NamespaceID slotID, NamespaceID armorID)
        {
            if (Armors == null)
                return null;
            return Armors.GetArmorModelAnchor(slotID, armorID);
        }
        public IEnumerable<string> GetAllArmorModelAnchors()
        {
            if (Armors == null)
                return null;
            return Armors.GetAllArmorModelAnchors();
        }
        public static ShapeMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            ShapeArmorMeta armors = null;
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "armors")
                {
                    armors = ShapeArmorMeta.FromXmlNode(child, defaultNsp);
                }
            }
            return new ShapeMeta()
            {
                ID = id,
                Armors = armors
            };
        }
    }
    public class ShapeArmorMeta
    {
        public ShapeArmorSlotMeta[] Slots { get; private set; }
        public Vector3 GetArmorPosition(NamespaceID slotID, NamespaceID armorID)
        {
            if (Slots == null)
                return Vector3.zero;
            foreach (var slot in Slots)
            {
                if (slot.SlotID != slotID)
                    continue;
                return slot.GetPosition(armorID);
            }
            return Vector3.zero;
        }
        public Vector3 GetArmorScale(NamespaceID slotID, NamespaceID armorID)
        {
            if (Slots == null)
                return Vector3.one;
            foreach (var slot in Slots)
            {
                if (slot.SlotID != slotID)
                    continue;
                return slot.GetScale(armorID);
            }
            return Vector3.one;
        }
        public Vector3 GetArmorModelOffset(NamespaceID slotID, NamespaceID armorID)
        {
            if (Slots == null)
                return Vector3.zero;
            foreach (var slot in Slots)
            {
                if (slot.SlotID != slotID)
                    continue;
                return slot.GetModelOffset(armorID);
            }
            return Vector3.zero;
        }
        public string GetArmorModelAnchor(NamespaceID slotID, NamespaceID armorID)
        {
            if (Slots == null)
                return null;
            foreach (var slot in Slots)
            {
                if (slot.SlotID != slotID)
                    continue;
                return slot.GetModelAnchor(armorID);
            }
            return null;
        }
        public IEnumerable<string> GetAllArmorModelAnchors()
        {
            if (Slots == null)
                yield break;
            foreach (var slot in Slots)
            {
                foreach (var anchor in slot.GetAllAnchors())
                {
                    yield return anchor;
                }
            }
        }
        public static ShapeArmorMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var slots = new List<ShapeArmorSlotMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "slot")
                {
                    slots.Add(ShapeArmorSlotMeta.FromXmlNode(child, defaultNsp));
                }
            }
            return new ShapeArmorMeta()
            {
                Slots = slots.ToArray(),
            };
        }
    }
    public class ShapeArmorSlotMeta
    {
        public NamespaceID SlotID { get; private set; }
        public ShapeArmorSlotMetaItem Default { get; private set; }
        public ShapeArmorSlotMetaItem[] Items { get; private set; }
        public Vector3 GetPosition(NamespaceID armorID)
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    if (item.ArmorID != armorID)
                        continue;
                    return item.Position;
                }
            }
            return Default?.Position ?? Vector3.zero;
        }
        public Vector3 GetScale(NamespaceID armorID)
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    if (item.ArmorID != armorID)
                        continue;
                    return item.Scale;
                }
            }
            return Default?.Scale ?? Vector3.one;
        }
        public Vector3 GetModelOffset(NamespaceID armorID)
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    if (item.ArmorID != armorID)
                        continue;
                    return item.ModelOffset;
                }
            }
            return Default?.ModelOffset ?? Vector3.zero;
        }
        public string GetModelAnchor(NamespaceID armorID)
        {
            if (Items != null)
            {
                foreach (var item in Items)
                {
                    if (item.ArmorID != armorID)
                        continue;
                    return item.ModelAnchor;
                }
            }
            return Default?.ModelAnchor;
        }
        public IEnumerable<string> GetAllAnchors()
        {
            if (Items != null)
            {
                if (!string.IsNullOrEmpty(Default?.ModelAnchor))
                {
                    yield return Default.ModelAnchor;
                }
                foreach (var item in Items)
                {
                    if (string.IsNullOrEmpty(item?.ModelAnchor))
                        continue;
                    yield return item.ModelAnchor;
                }
            }
        }
        public static ShapeArmorSlotMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var slotID = node.GetAttributeNamespaceID("id", defaultNsp);

            var defaultNode = node["default"];
            ShapeArmorSlotMetaItem defaultItem = null;
            if (defaultNode != null)
            {
                defaultItem = ShapeArmorSlotMetaItem.FromXmlNode(defaultNode, defaultNsp, null);
            }

            var items = new List<ShapeArmorSlotMetaItem>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "custom")
                {
                    items.Add(ShapeArmorSlotMetaItem.FromXmlNode(child, defaultNsp, defaultItem));
                }
            }
            return new ShapeArmorSlotMeta()
            {
                SlotID = slotID,
                Default = defaultItem,
                Items = items.ToArray(),
            };
        }
    }
    public class ShapeArmorSlotMetaItem
    {
        public NamespaceID ArmorID { get; private set; }
        public Vector3 Position { get; private set; }
        public Vector3 Scale { get; private set; }
        public Vector3 ModelOffset { get; private set; }
        public string ModelAnchor { get; private set; }
        public static ShapeArmorSlotMetaItem FromXmlNode(XmlNode node, string defaultNsp, ShapeArmorSlotMetaItem defaultItem)
        {
            var armorID = node.GetAttributeNamespaceID("id", defaultNsp);
            var position = node["position"]?.GetAttributeVector3() ?? defaultItem?.Position ?? Vector3.zero;
            var scale = node["scale"]?.GetAttributeVector3() ?? defaultItem?.Scale ?? Vector3.one;
            var modelNode = node["model"];
            var modelOffset = modelNode?["offset"]?.GetAttributeVector3() ?? defaultItem?.ModelOffset ?? Vector3.zero;
            var modelAnchor = modelNode?["anchor"]?.GetAttribute("value") ?? defaultItem?.ModelAnchor;

            return new ShapeArmorSlotMetaItem()
            {
                ArmorID = armorID,
                Position = position,
                Scale = scale,
                ModelOffset = modelOffset,
                ModelAnchor = modelAnchor
            };
        }
    }
}
