using MVZ2.Level;
using MVZ2.Models;
using PVZEngine;

namespace MVZ2.HeldItems
{
    public class HeldItemModelInterface : ModelInterface
    {
        public HeldItemModelInterface(LevelController level)
        {
            this.level = level;
        }
        public override void ChangeModel(NamespaceID modelID)
        {
            level.SetHeldItemModel(modelID);
        }
        protected override Model GetModel()
        {
            return level.GetHeldItemModel();
        }

        private LevelController level;
    }
}
