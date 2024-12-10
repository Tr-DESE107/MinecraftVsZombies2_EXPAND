using MVZ2.Models;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Models;
using UnityEngine;

namespace MVZ2.Entities
{
    public class ArmorModelInterface : EntityModelInterface
    {
        public ArmorModelInterface(EntityController ctrl) : base(ctrl) { }
        public override void ChangeModel(NamespaceID modelID)
        {
            controller.ChangeArmorModel(modelID);
        }
        protected override Model GetModel()
        {
            return controller.Model.ArmorModel;
        }
    }
}
