using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Level.UI;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    public abstract class BlueprintDisplayer : MonoBehaviour
    {
        public void SetCommandBlockActive(bool value)
        {
            commandBlockRoot.SetActive(value);
        }
        public abstract void UpdateItems(ChoosingBlueprintViewData[] viewDatas);
        public abstract Blueprint GetItem(int index);
        private void Awake()
        {
            commandBlockBlueprint.OnPointerDown += (blueprint, eventData) => OnCommandBlockBlueprintClick?.Invoke();
        }
        protected void CallBlueprintPointerEnter(int index, PointerEventData eventData)
        {
            OnBlueprintPointerEnter?.Invoke(index, eventData);
        }
        protected void CallBlueprintPointerExit(int index, PointerEventData eventData)
        {
            OnBlueprintPointerExit?.Invoke(index, eventData);
        }
        protected void CallBlueprintPointerDown(int index, PointerEventData eventData)
        {
            OnBlueprintPointerDown?.Invoke(index, eventData);
        }
        public event Action OnCommandBlockBlueprintClick;
        public event Action<int, PointerEventData> OnBlueprintPointerEnter;
        public event Action<int, PointerEventData> OnBlueprintPointerExit;
        public event Action<int, PointerEventData> OnBlueprintPointerDown;
        [SerializeField]
        GameObject commandBlockRoot;
        [SerializeField]
        Blueprint commandBlockBlueprint;
    }
    public struct ChoosingBlueprintViewData
    {
        public BlueprintViewData blueprint;
        public bool disabled;
        public bool selected;

        public static readonly ChoosingBlueprintViewData Empty = new ChoosingBlueprintViewData()
        {
            blueprint = BlueprintViewData.Empty
        };
    }
}
