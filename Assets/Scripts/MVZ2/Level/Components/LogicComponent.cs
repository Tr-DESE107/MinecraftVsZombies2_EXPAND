using System;
using log4net.Core;
using MVZ2.GameContent;
using MVZ2.Level;
using PVZEngine.Definitions;

namespace PVZEngine.Level
{
    public partial class LogicComponent : MVZ2Component
    {
        public LogicComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void BeginLevel(string transition)
        {
            Controller.BeginLevel(transition);
        }
        public void StopLevel()
        {
            Controller.StopLevel();
        }
        public static readonly NamespaceID componentID = new NamespaceID(Builtin.spaceName, "logic");
    }
}