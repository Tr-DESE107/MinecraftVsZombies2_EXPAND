using MVZ2Logic.Games;
using PVZEngine;

namespace MVZ2.Games
{
    public partial class Game : IGame
    {
        public Game(string defaultNsp, IGameLocalization localization)
        {
            DefaultNamespace = defaultNsp;
            this.localization = localization;
        }
        public void SetProperty<T>(PropertyKey<T> name, T value) => propertyDict.SetProperty<T>(name, value);
        public T GetProperty<T>(PropertyKey<T> name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public IPropertyKey[] GetPropertyNames() => propertyDict.GetPropertyNames();
        public string DefaultNamespace { get; private set; }
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
