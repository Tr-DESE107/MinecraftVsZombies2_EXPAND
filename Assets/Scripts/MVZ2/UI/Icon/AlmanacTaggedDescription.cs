using System;
using TMPro;
using UnityEngine;

namespace MVZ2.UI
{
    public class AlmanacTaggedDescription : MonoBehaviour
    {
        public void UpdateIconStacks(AlmanacDescriptionTagViewData[] infos)
        {
            tmpText.ForceMeshUpdate();
            iconStackList.updateList(infos.Length, (i, obj) =>
            {
                var container = obj.GetComponent<AlmanacDescriptionTag>();
                if (!container)
                    return;
                var viewData = infos[i];
                var linkID = viewData.linkID;
                // 查找占位符位置
                int linkIndex = FindLinkIndexByID(linkID);
                if (linkIndex == -1)
                    return;

                TMP_LinkInfo linkInfo = tmpText.textInfo.linkInfo[linkIndex];
                Vector3 centerPos = CalculateLinkCenter(linkInfo);
                Vector3 worldPos = tmpText.transform.TransformPoint(centerPos);
                obj.name = linkID;
                obj.transform.position = worldPos;
                container.UpdateTag(viewData);
            },
            obj =>
            {
                var container = obj.GetComponent<AlmanacDescriptionTag>();
                container.OnPointerEnter += OnIconPointerEnterCallback;
                container.OnPointerExit += OnIconPointerExitCallback;
            },
            obj =>
            {
                var container = obj.GetComponent<AlmanacDescriptionTag>();
                container.OnPointerEnter -= OnIconPointerEnterCallback;
                container.OnPointerExit -= OnIconPointerExitCallback;
            });
        }
        public AlmanacTagIcon GetIconContainer(string linkID)
        {
            var index = FindLinkIndexByID(linkID);
            if (index < 0)
                return null;
            return iconStackList.getElement<AlmanacDescriptionTag>(index).icon;
        }
        private int FindLinkIndexByID(string linkID)
        {
            var textInfo = tmpText.textInfo;
            if (textInfo == null)
                return -1;
            for (int i = 0; i < textInfo.linkCount; i++)
            {
                if (textInfo.linkInfo[i].GetLinkID() == linkID)
                {
                    return i;
                }
            }
            return -1;
        }
        private Vector3 CalculateLinkCenter(TMP_LinkInfo linkInfo)
        {
            Vector3 bottomLeft = tmpText.textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex].bottomLeft;
            Vector3 topRight = tmpText.textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength - 1].topRight;

            return new Vector3((bottomLeft.x + topRight.x) / 2, (bottomLeft.y + topRight.y) / 2, 0);
        }
        private void OnIconPointerEnterCallback(string linkID)
        {
            OnIconEnter?.Invoke(linkID);
        }
        private void OnIconPointerExitCallback(string linkID)
        {
            OnIconExit?.Invoke(linkID);
        }

        #region 事件
        public event Action<string> OnIconEnter;
        public event Action<string> OnIconExit;
        #endregion

        #region 属性字段
        [SerializeField]
        private ElementList iconStackList;
        [SerializeField]
        private TextMeshProUGUI tmpText;
        #endregion

    }
}