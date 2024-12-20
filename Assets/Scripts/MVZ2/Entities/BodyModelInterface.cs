using MVZ2.Models;

namespace MVZ2.Entities
{
    public class BodyModelInterface : EntityModelInterface
    {
        public BodyModelInterface(EntityController ctrl) : base(ctrl) { }
        protected override Model GetModel()
        {
            return controller.Model;
        }
    }
}
