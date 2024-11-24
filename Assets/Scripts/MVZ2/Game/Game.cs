using MVZ2Logic.Games;
using MVZ2Logic.Saves;
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

        public void SetProperty(string name, object value) => propertyDict.SetProperty(name, value);
        public object GetProperty(string name) => propertyDict.GetProperty(name);
        public bool TryGetProperty(string name, out object value) => propertyDict.TryGetProperty(name, out value);
        public T GetProperty<T>(string name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(string name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public string[] GetPropertyNames() => propertyDict.GetPropertyNames();
        public string DefaultNamespace { get; private set; }
        private LevelEngine level;
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
