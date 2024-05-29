using System.Collections.Generic;
using System.Text;
using MukioI18n;
using UnityEngine;

namespace MVZ2.Localization
{
    public abstract class TranslateComponent<T> : MonoBehaviour, ITranslateComponent where T : Component
    {
        private void OnEnable()
        {
            lang.OnLanguageChanged += OnLanguageChangedCallback;
            Translate(lang.GetCurrentLanguage());
        }
        private void OnDisable()
        {
            lang.OnLanguageChanged -= OnLanguageChangedCallback;
        }
        protected virtual void Translate(string language)
        {

        }
        protected virtual string GetKeyInner()
        {
            return null;
        }
        protected virtual IEnumerable<string> GetKeysInner()
        {
            return null;
        }
        private void OnLanguageChangedCallback(string language)
        {
            Translate(language);
        }
        public T Component
        {
            get
            {
                if (!component)
                {
                    component = GetComponent<T>();
                }
                return component;
            }
        }
        public string Context => context;
        public string Comment => comment;
        public string Key
        {
            get
            {
                if (key == null)
                    key = GetKeyInner();
                return key;
            }
        }
        public IEnumerable<string> Keys
        {
            get
            {
                if (keys == null)
                    keys = GetKeysInner();
                return keys;
            }
        }
        public virtual string Path
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                Transform tr = transform;
                do
                {
                    sb.Insert(0, tr.name);
                    sb.Insert(0, "/");
                } while (tr = tr.parent);

                return sb.ToString();
            }
        }
        protected LanguageManager lang => MainManager.Instance.LanguageManager;
        [SerializeField]
        private string context;
        [SerializeField]
        private string comment;
        private T component;
        private string key;
        private IEnumerable<string> keys;
    }
}
