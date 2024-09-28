using MVZ2.GameContent;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class LogicComponent : MVZ2Component, ILogicComponent
    {
        public LogicComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void BeginLevel()
        {
            Controller.BeginLevel();
        }
        public void StopLevel()
        {
            Controller.StopLevel();
        }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "logic");
    }
}