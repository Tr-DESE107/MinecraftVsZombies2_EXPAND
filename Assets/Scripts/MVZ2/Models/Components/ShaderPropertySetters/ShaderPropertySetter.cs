using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(RendererElement))]
    public abstract class ShaderPropertySetter<T> : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            UpdateProperty();
        }
        public void ResetProperty()
        {
            SetProperty(GetDefaultValue());
        }
        public void UpdateProperty()
        {
            SetProperty(GetCurrentValue());
        }
        public abstract void SetProperty(T value);
        public abstract T GetDefaultValue();
        public abstract T GetCurrentValue();
        protected virtual void OnEnable()
        {
            UpdateProperty();
        }
        protected virtual void OnDisable()
        {
            ResetProperty();
        }
        public RendererElement Element
        {
            get
            {
                if (!element)
                {
                    element = GetComponent<RendererElement>();
                }
                return element;
            }
        }
        private RendererElement element;
    }
}
