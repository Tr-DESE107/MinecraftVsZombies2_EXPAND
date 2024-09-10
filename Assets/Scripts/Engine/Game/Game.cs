using System;
using PVZEngine.Base;
using PVZEngine.LevelManaging;

namespace PVZEngine.Game
{
    public partial class Game : IGame
    {
        public Game()
        {
        }
        public bool IsInLevel()
        {
            return GetLevel() != null;
        }
        public Level GetLevel()
        {
            return level;
        }
        public void SetLevel(Level value)
        {
            level = value;
        }
        public void SetProperty(string name, object value) => propertyDict.SetProperty(name, value);
        public object GetProperty(string name) => propertyDict.GetProperty(name);
        public bool TryGetProperty(string name, out object value) => propertyDict.TryGetProperty(name, out value);
        public T GetProperty<T>(string name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(string name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public string[] GetPropertyNames() => propertyDict.GetPropertyNames();
        public string GetText(string textKey)
        {
            return OnGetString?.Invoke(textKey) ?? textKey;
        }
        public string GetTextParticular(string textKey, string context)
        {
            return OnGetStringParticular?.Invoke(textKey, context) ?? textKey;
        }
        public event Func<string, string> OnGetString;
        public event Func<string, string, string> OnGetStringParticular;
        private Level level;
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
