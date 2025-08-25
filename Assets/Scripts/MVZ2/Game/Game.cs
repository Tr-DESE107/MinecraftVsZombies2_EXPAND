using MVZ2Logic.Games;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Games
{
    public partial class Game : IGame
    {
        public Game(string defaultNsp, IGameLocalization localization, IGameSaveData saveDataProvider)
        {
            DefaultNamespace = defaultNsp;
            this.localization = localization;
            this.saveDataProvider = saveDataProvider;
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
        public string DefaultNamespace { get; private set; }
        private LevelEngine level;
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
