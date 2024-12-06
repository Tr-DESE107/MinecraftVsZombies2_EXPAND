using System;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Almanacs
{
    public class AlmanacEntry : MonoBehaviour
    {
        public void UpdateEntry(AlmanacEntryViewData viewData)
        {
            rootObject.SetActive(!viewData.empty);
            icon.sprite = viewData.sprite;
            icon.enabled = icon.sprite;
        }
        private void Awake()
        {
            button.onClick.AddListener(() => OnClick?.Invoke(this));
        }
        public Action<AlmanacEntry> OnClick;
        [SerializeField]
        private GameObject rootObject;
        [SerializeField]
        private Button button;
        [SerializeField]
        private Image icon;
    }
    public struct AlmanacEntryViewData
    {
        public bool empty;
        public Sprite sprite;
        public static readonly AlmanacEntryViewData Empty = new AlmanacEntryViewData { empty = true };
    }
}
