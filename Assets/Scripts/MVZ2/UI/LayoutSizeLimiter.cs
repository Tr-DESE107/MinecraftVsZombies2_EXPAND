using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MVZ2.UI
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class LayoutSizeLimiter : UIBehaviour, ILayoutElement
    {
        protected LayoutSizeLimiter()
        { }
        protected override void OnEnable()
        {
            base.OnEnable();
            SetDirty();
        }
        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }
        protected override void OnRectTransformDimensionsChange()
        {
            SetDirty();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }
#endif
        protected void SetDirty()
        {
            if (!IsActive())
                return;

            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        }
        public virtual void CalculateLayoutInputHorizontal()
        {
            if (maxWidth < 0)
            {
                _currentWidth = -1;
                return;
            }
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaX);

            _disabled = true;
            var preferredWidth = LayoutUtility.GetPreferredSize(m_Rect, 0);
            var width = Mathf.Min(preferredWidth, maxWidth);
            _currentWidth = width;
            _disabled = false;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }
        public virtual void CalculateLayoutInputVertical()
        {
            if (maxHeight < 0)
            {
                _currentHeight = -1;
                return;
            }
            m_Tracker.Add(this, rectTransform, DrivenTransformProperties.SizeDeltaY);

            _disabled = true;
            var preferredHeight = LayoutUtility.GetPreferredSize(m_Rect, 1);
            var height = Mathf.Min(preferredHeight, maxHeight);
            _currentHeight = height;
            _disabled = false;

            // Set size to min or preferred size
            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        [System.NonSerialized]
        private RectTransform m_Rect;
        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }
        [SerializeField]
        private float maxWidth = -1;
        [SerializeField]
        private float maxHeight = -1;
        private bool _disabled;
        private float _currentWidth;
        private float _currentHeight;
        private DrivenRectTransformTracker m_Tracker;
        public float minWidth => -1;
        public float minHeight => -1;
        public float preferredWidth => _disabled ? -1 : _currentWidth;
        public float preferredHeight => _disabled ? -1 : _currentHeight;
        public float flexibleWidth => -1;
        public float flexibleHeight => -1;
        public int layoutPriority => 100;
    }
}
