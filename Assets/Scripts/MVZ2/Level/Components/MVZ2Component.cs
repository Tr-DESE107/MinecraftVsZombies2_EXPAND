using MVZ2.Managers;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Level.Components
{
    public abstract class MVZ2Component : LevelComponent
    {
        public MVZ2Component(LevelEngine level, NamespaceID id, LevelController controller) : base(level, id)
        {
            Controller = controller;
        }
        public override ISerializableLevelComponent ToSerializable()
        {
            return new EmptySerializableLevelComponent();
        }
        public override void LoadSerializable(ISerializableLevelComponent seri)
        {
        }
        public virtual void PostLevelLoad()
        {

        }
        protected MainManager Main => MainManager.Instance;
        public LevelController Controller { get; private set; }
    }
}
