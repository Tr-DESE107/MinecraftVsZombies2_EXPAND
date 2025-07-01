using MVZ2.Vanilla;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;
using PVZEngine.Models;

namespace MVZ2.Level.Components
{
    public partial class AreaComponent : MVZ2Component, IAreaComponent
    {
        public AreaComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public IModelInterface GetAreaModelInterface()
        {
            return Controller.GetAreaModelInterface();
        }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "area");
    }
}