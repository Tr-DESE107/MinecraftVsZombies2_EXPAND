using MVZ2.Models;
using PVZEngine;
using PVZEngine.Models;

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
