namespace PVZEngine.Level
{
    public abstract class LevelComponent
    {
        public LevelComponent(LevelEngine level, NamespaceID id)
        {
            Level = level;
            this.id = id;
        }
        public virtual void Update() { }
        public abstract ISerializableLevelComponent ToSerializable();
        public abstract void LoadSerializable(ISerializableLevelComponent seri);

        public NamespaceID GetID() => id;
        public LevelEngine Level { get; private set; }
        private NamespaceID id;
    }
}
