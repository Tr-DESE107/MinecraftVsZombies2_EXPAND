using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level
{
    public abstract class MVZ2Component : LevelComponent
    {
        public MVZ2Component(LevelEngine level, NamespaceID id, LevelController controller) : base(level, id)
        {
            Controller = controller;
        }
        protected MainManager Main => MainManager.Instance;
        public LevelController Controller { get; private set; }
    }
}
