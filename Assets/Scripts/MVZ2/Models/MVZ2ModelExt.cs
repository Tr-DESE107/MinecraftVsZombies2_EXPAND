using MVZ2Logic;
using PVZEngine;
using PVZEngine.Armors;

namespace MVZ2.Models
{
    public static class MVZ2ModelExt
    {
        public static string GetAnchorOfArmorSlot(NamespaceID slot)
        {
            var game = Global.Game;
            var slotMeta = game.GetArmorSlotMeta(slot);
            if (slotMeta == null)
                return null;
            return slotMeta.Anchor;
        }
        public static void CreateArmor(this Model model, string anchor, NamespaceID slot, NamespaceID id)
        {
            var key = EngineArmorExt.GetModelKeyOfArmorSlot(slot);
            model.CreateChildModel(anchor, key, id);
        }
        public static bool RemoveArmor(this Model model, NamespaceID slot)
        {
            var key = EngineArmorExt.GetModelKeyOfArmorSlot(slot);
            return model.RemoveChildModel(key);
        }
        public static Model GetArmorModel(this Model model, NamespaceID slot)
        {
            var key = EngineArmorExt.GetModelKeyOfArmorSlot(slot);
            return model.GetChildModel(key);
        }
        public static void ClearArmorModel(this Model model, NamespaceID slot)
        {
            var anchor = GetAnchorOfArmorSlot(slot);
            model.ClearModelAnchor(anchor);
        }
    }
}
