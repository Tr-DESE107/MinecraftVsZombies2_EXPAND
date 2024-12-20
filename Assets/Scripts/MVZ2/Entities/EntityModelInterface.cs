using MVZ2.Models;

namespace MVZ2.Entities
{
    public abstract class EntityModelInterface : ModelInterface
    {
        public EntityModelInterface(EntityController ctrl)
        {
            this.controller = ctrl;
        }
        protected EntityController controller;
    }
}
