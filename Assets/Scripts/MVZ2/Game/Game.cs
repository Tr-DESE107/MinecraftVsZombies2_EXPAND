using MVZ2.Vanilla;
using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Games
{
    public partial class Game : IGame
    {
        public Game(string defaultNsp, IGameLocalization localization, IGameSaveData saveDataProvider, IGameMetas metaProvider)
        {
            DefaultNamespace = defaultNsp;
            this.localization = localization;
            this.saveDataProvider = saveDataProvider;
            this.metaProvider = metaProvider;
        }
        public bool IsInLevel()
        {
            return GetLevel() != null;
        }
        public LevelEngine GetLevel()
        {
            return level;
        }
        public void SetLevel(LevelEngine value)
        {
            level = value;
        }

        public void SetProperty<T>(PropertyKey<T> name, T value) => propertyDict.SetProperty<T>(name, value);
        public T GetProperty<T>(PropertyKey<T> name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public IPropertyKey[] GetPropertyNames() => propertyDict.GetPropertyNames();

        public string GetEntityName(NamespaceID entityID)
        {
            if (entityID == null)
                return "null";
            var meta = GetEntityMeta(entityID);
            if (meta == null)
                return entityID.ToString();
            var name = meta.Name ?? VanillaStrings.UNKNOWN_ENTITY_NAME;
            return GetTextParticular(name, VanillaStrings.CONTEXT_ENTITY_NAME);
        }
        public string GetEntityCounterName(NamespaceID counterID)
        {
            if (counterID == null)
                return "null";
            var meta = GetEntityCounterMeta(counterID);
            if (meta == null)
                return counterID.ToString();
            var name = meta.Name ?? VanillaStrings.UNKNOWN_ENTITY_COUNTER_NAME;
            return GetTextParticular(name, VanillaStrings.CONTEXT_ENTITY_COUNTER_NAME);
        }
        public string DefaultNamespace { get; private set; }
        private LevelEngine level;
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
