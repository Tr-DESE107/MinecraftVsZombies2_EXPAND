using UnityEngine;

namespace MVZ2.Localization
{
    public class TranslateComponent<T> : MonoBehaviour where T : Component
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
        protected LanguageManager lang => MainManager.Instance.LanguageManager;
        [SerializeField]
        private string context;
        [SerializeField]
        private string comment;
        private T component;
    }
}
