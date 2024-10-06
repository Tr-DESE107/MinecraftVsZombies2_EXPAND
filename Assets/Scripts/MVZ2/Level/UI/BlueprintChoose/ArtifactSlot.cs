using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Level.UI
{
    public class ArtifactSlot : MonoBehaviour
    {
        public void ResetView()
        {
            SetSprite(null);
        }
        public void UpdateView(ArtifactViewData viewData)
        {
            SetSprite(viewData.sprite);
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        private void SetSprite(Sprite sprite)
        {
            image.sprite = sprite;
            image.enabled = sprite;
        }
        public event Action<ArtifactSlot> OnClick;
        [SerializeField]
        Image image;
        [SerializeField]
        Button button;
    }
    public struct ArtifactViewData
    {
        public Sprite sprite;
    }
}
