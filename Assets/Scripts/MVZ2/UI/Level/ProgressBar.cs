﻿using MVZ2.Metas;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ProgressBar : MonoBehaviour
    {
        public void UpdateTemplate(ProgressBarTemplateViewData viewData)
        {
            layoutElement.minWidth = viewData.size.x;
            layoutElement.minHeight = viewData.size.y;
            backgroundImage.sprite = viewData.backgroundSprite;
            foregroundImage.sprite = viewData.foregroundSprite;
            foregroundImage.enabled = foregroundImage.sprite;

            barImage.sprite = viewData.barSprite;
            barImage.type = viewData.barMode == ProgressBarMode.Filled ? Image.Type.Filled : Image.Type.Sliced;
            barImage.fillMethod = Image.FillMethod.Horizontal;
            barImage.fillOrigin = (int)(viewData.fromLeft ? Image.OriginHorizontal.Left : Image.OriginHorizontal.Right);

            iconImage.sprite = viewData.iconSprite;
            iconImage.enabled = iconImage.sprite;
            slider.direction = viewData.fromLeft ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;


            var padding = viewData.padding;
            var left = padding.x;
            var bottom = padding.y;
            var right = padding.z;
            var top = padding.w;
            var offset = new Vector2((left - right) * 0.5f, (bottom - top) * 0.5f);
            var size = new Vector2(-left - right, -bottom - top);
            barRegion.anchoredPosition = offset;
            barRegion.sizeDelta = size;
        }
        public void SetProgress(float progress)
        {
            slider.SetValueWithoutNotify(progress);
        }
        public void SetBannerProgresses(float[] progresses)
        {
            flags.updateList(progresses.Length, (i, rect) =>
            {
                var component = rect.GetComponent<ProgressBarBanner>();
                component.SetRiseProgress(progresses[i]);
            });
        }

        [SerializeField]
        private LayoutElement layoutElement;
        [SerializeField]
        private ElementListUI flags;
        [SerializeField]
        private Image foregroundImage;
        [SerializeField]
        private Image backgroundImage;
        [SerializeField]
        private Image barImage;
        [SerializeField]
        private Image iconImage;
        [SerializeField]
        private RectTransform barRegion;
        [SerializeField]
        private Slider slider;
    }
    public struct ProgressBarTemplateViewData
    {
        public Vector2 size;
        public Sprite backgroundSprite;
        public Sprite foregroundSprite;
        public Sprite barSprite;
        public Sprite iconSprite;
        public bool fromLeft;
        public ProgressBarMode barMode;
        public Vector4 padding;
    }
}
