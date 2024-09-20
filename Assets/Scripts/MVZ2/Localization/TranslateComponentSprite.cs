using UnityEngine;

namespace MVZ2.Localization
{
    public abstract class TranslateComponentSprite<T> : TranslateComponent<T> where T : Component
    {
        protected virtual Sprite GetKeyInner() => null;
        public Sprite Key
        {
            get
            {
                if (key == null)
                    key = GetKeyInner();
                return key;
            }
        }
        private Sprite key;
    }
}
