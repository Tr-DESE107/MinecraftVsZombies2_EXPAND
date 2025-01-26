using MVZ2Logic.Models;
using PVZEngine;

namespace MVZ2.Models
{
    public static class MVZ2ModelExt
    {
        public static void CreateArmor(this Model model, NamespaceID id)
        {
            model.CreateChildModel(LogicModelHelper.ANCHOR_ARMOR, LogicModelHelper.KEY_ARMOR, id);
        }
        public static bool RemoveArmor(this Model model)
        {
            return model.RemoveChildModel(LogicModelHelper.KEY_ARMOR);
        }
        public static Model GetArmorModel(this Model model)
        {
            return model.GetChildModel(LogicModelHelper.KEY_ARMOR);
        }
        public static void ClearArmorModel(this Model model)
        {
            model.ClearModelAnchor(LogicModelHelper.ANCHOR_ARMOR);
        }
    }
}
