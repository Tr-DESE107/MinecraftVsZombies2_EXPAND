using MVZ2.Vanilla;
using MVZ2Logic.Level.Components;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public partial class TalkComponent : MVZ2Component, ITalkComponent
    {
        public TalkComponent(LevelEngine level, LevelController controller) : base(level, componentID, controller)
        {
        }
        public void StartTalk(NamespaceID id, int section, float delay = 1)
        {
            Controller.StartTalk(id, section, delay);
        }
        public static readonly NamespaceID componentID = new NamespaceID(VanillaMod.spaceName, "talk");
    }
}