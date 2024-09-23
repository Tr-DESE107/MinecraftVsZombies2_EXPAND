using MVZ2.Resources;
using MVZ2.Save;
using PVZEngine;
using PVZEngine.Level;

namespace MVZ2.Games
{
    public partial class Game : IGame
    {
        public Game(ITranslator translator, ISaveDataProvider saveDataProvider, IMetaProvider metaProvider)
        {
            this.translator = translator;
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

        public string GetText(string textKey, params string[] args)
        {
            return translator.GetText(textKey, args);
        }

        public string GetTextParticular(string textKey, string context, params string[] args)
        {
            return translator.GetTextParticular(textKey, context, args);
        }

        public bool IsUnlocked(NamespaceID unlockID)
        {
            return saveDataProvider.IsUnlocked(unlockID);
        }

        public void Unlock(NamespaceID unlockID)
        {
            saveDataProvider.Unlock(unlockID);
        }

        public T GetModSaveData<T>(string spaceName)
        {
            return saveDataProvider.GetModSaveData<T>(spaceName);
        }

        public ModSaveData GetModSaveData(string spaceName)
        {
            return saveDataProvider.GetModSaveData(spaceName);
        }

        public void SaveCurrentModData(string spaceName)
        {
            saveDataProvider.SaveCurrentModData(spaceName);
        }
        public void SetProperty(string name, object value) => propertyDict.SetProperty(name, value);
        public object GetProperty(string name) => propertyDict.GetProperty(name);
        public bool TryGetProperty(string name, out object value) => propertyDict.TryGetProperty(name, out value);
        public T GetProperty<T>(string name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(string name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public string[] GetPropertyNames() => propertyDict.GetPropertyNames();
        private LevelEngine level;
        private PropertyDictionary propertyDict = new PropertyDictionary();
        private ITranslator translator;
        private ISaveDataProvider saveDataProvider;
    }
}
