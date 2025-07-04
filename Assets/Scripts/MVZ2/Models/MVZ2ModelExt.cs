using System.Linq;
using MVZ2Logic;
using PVZEngine;
using PVZEngine.Armors;

namespace MVZ2.Models
{
    public static class MVZ2ModelExt
    {
        public static string[] GetAnchorsOfArmorSlot(NamespaceID slot)
        {
            var game = Global.Game;
            var slotMeta = game.GetArmorSlotMeta(slot);
            if (slotMeta == null)
                return null;
            return slotMeta.Anchors.Select(a => a.Anchor).ToArray();
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
    }
}
