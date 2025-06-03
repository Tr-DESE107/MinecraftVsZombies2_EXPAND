using System;
using UnityEngine;

namespace MVZ2.UI
{
    public class AlmanacDescriptionTag : MonoBehaviour
    {
        public void UpdateTag(AlmanacDescriptionTagViewData viewData)
        {
            linkID = viewData.linkID;
            icon.UpdateContainer(viewData.icon);
        }
        private void Awake()
        {
            icon.OnPointerEnter += _ => OnPointerEnter?.Invoke(linkID);
            icon.OnPointerExit += _ => OnPointerExit?.Invoke(linkID);
        }
        public event Action<string> OnPointerEnter;
        public event Action<string> OnPointerExit;
        public string linkID;
        public AlmanacTagIcon icon;
    }
    public struct AlmanacDescriptionTagViewData
    {
        public string linkID;
        public AlmanacTagIconViewData icon;
    }
}
