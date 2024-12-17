using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2Logic.Models;
using PVZEngine;
using UnityEngine;

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
    }
}
