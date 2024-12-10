using MVZ2.Models;
using PVZEngine;

namespace MVZ2.Entities
{
    public class BodyModelInterface : EntityModelInterface
    {
        public BodyModelInterface(EntityController ctrl) : base(ctrl) { }
        public override void ChangeModel(NamespaceID modelID)
        {
            controller.SetModel(modelID);
        }
        protected override Model GetModel()
        {
            return controller.Model;
        }
    }
}
