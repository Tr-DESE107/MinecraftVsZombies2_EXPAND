﻿using System;
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
                Vector2 linkSize = CalculateLinkSize(linkInfo);
                Vector3 worldPos = tmpText.transform.TransformPoint(centerPos);
                obj.name = linkID;
                obj.transform.position = worldPos;
                var xScale = linkSize.x / viewData.size.x;
                var yScale = linkSize.y / viewData.size.y;
                var scale = Mathf.Min(xScale, yScale);
                container.UpdateTag(viewData);
                container.SetScale(Vector3.one * scale);
            },
            obj =>
            {
                var container = obj.GetComponent<AlmanacDescriptionTag>();
                container.OnPointerEnter += OnIconPointerEnterCallback;
                container.OnPointerExit += OnIconPointerExitCallback;
                container.OnPointerDown += OnIconPointerDownCallback;
            },
            obj =>
            {
                var container = obj.GetComponent<AlmanacDescriptionTag>();
                container.OnPointerEnter -= OnIconPointerEnterCallback;
                container.OnPointerExit -= OnIconPointerExitCallback;
                container.OnPointerDown -= OnIconPointerDownCallback;
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
        private Vector2 CalculateLinkSize(TMP_LinkInfo linkInfo)
        {
            Vector3 bottomLeft = tmpText.textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex].bottomLeft;
            Vector3 topRight = tmpText.textInfo.characterInfo[linkInfo.linkTextfirstCharacterIndex + linkInfo.linkTextLength - 1].topRight;

            return new Vector2(Mathf.Abs(topRight.x - bottomLeft.x), Mathf.Abs(topRight.y - bottomLeft.y));
        }
        private void OnIconPointerEnterCallback(string linkID)
        {
            OnIconEnter?.Invoke(linkID);
        }
        private void OnIconPointerExitCallback(string linkID)
        {
            OnIconExit?.Invoke(linkID);
        }
        private void OnIconPointerDownCallback(string linkID)
        {
            OnIconDown?.Invoke(linkID);
        }

        #region 事件
        public event Action<string> OnIconEnter;
        public event Action<string> OnIconExit;
        public event Action<string> OnIconDown;
        #endregion

        #region 属性字段
        [SerializeField]
        private ElementList iconStackList;
        [SerializeField]
        private TextMeshProUGUI tmpText;
        #endregion

    }
}