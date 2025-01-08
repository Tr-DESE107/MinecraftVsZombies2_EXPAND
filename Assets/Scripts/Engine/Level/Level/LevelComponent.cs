namespace PVZEngine.Level
{
    public interface ILevelComponent
    {
        NamespaceID GetID();
        void OnStart();
        void Update();
        ISerializableLevelComponent ToSerializable();
        void LoadSerializable(ISerializableLevelComponent seri);
    }
    public abstract class LevelComponent : ILevelComponent
    {
        public LevelComponent(LevelEngine level, NamespaceID id)
        {
            Level = level;
            this.id = id;
        }
        public virtual void OnStart() { }
        public virtual void Update() { }
        public abstract ISerializableLevelComponent ToSerializable();
        public abstract void LoadSerializable(ISerializableLevelComponent seri);

        public NamespaceID GetID() => id;
        public LevelEngine Level { get; private set; }
        private NamespaceID id;
    }
}
