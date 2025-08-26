using System.Collections;
using MVZ2.Managers;
using MVZ2Logic.Games;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Games
{
    public partial class Game : IGame
    {
        public Game(MainManager main, IGameLocalization localization)
        {
            this.localization = localization;
        }
        public bool IsMobile()
        {
            return main.IsMobile();
        }
        public Coroutine StartCoroutine(IEnumerator enumerator)
        {
            return main.CoroutineManager.StartCoroutine(enumerator);
        }

        public void SetProperty<T>(PropertyKey<T> name, T value) => propertyDict.SetProperty<T>(name, value);
        public T GetProperty<T>(PropertyKey<T> name) => propertyDict.GetProperty<T>(name);
        public bool TryGetProperty<T>(PropertyKey<T> name, out T value) => propertyDict.TryGetProperty<T>(name, out value);
        public IPropertyKey[] GetPropertyNames() => propertyDict.GetPropertyNames();
        public string DefaultNamespace => main.BuiltinNamespace;
        private MainManager main;
        private PropertyDictionary propertyDict = new PropertyDictionary();
    }
}
