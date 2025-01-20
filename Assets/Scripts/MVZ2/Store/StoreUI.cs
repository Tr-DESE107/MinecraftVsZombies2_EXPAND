using System;
using MVZ2.Talk;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Store
{
    public class StoreUI : MonoBehaviour
    {
        public void Display()
        {
            speechBubble.SetDirection(SpeechBubbleDirection.Left);
            character.SetSpeaking(true);
            character.ResetMotion();
        }
        public void SetStoreUIVisible(bool visible)
        {
            storeUIRoot.SetActive(visible);
            speechBubble.SetDirection(SpeechBubbleDirection.Left);
        }
        public void SetMoney(string money)
        {
            moneyPanel.SetMoney(money);
        }
        public void SetProducts(ProductItemViewData[] viewDatas)
        {
            products.updateList(viewDatas.Length, (i, obj) =>
            {
                var page = obj.GetComponent<StoreProductItem>();
                page.UpdateItem(viewDatas[i]);
            },
            obj =>
            {
                var page = obj.GetComponent<StoreProductItem>();
                page.OnClick += OnPageProductClickCallback;
                page.OnPointerEnter += OnPageProductPointerEnterCallback;
                page.OnPointerExit += OnPageProductPointerExitCallback;
            },
            obj =>
            {
                var page = obj.GetComponent<StoreProductItem>();
                page.OnClick -= OnPageProductClickCallback;
                page.OnPointerEnter -= OnPageProductPointerEnterCallback;
                page.OnPointerExit -= OnPageProductPointerExitCallback;
            });
        }
        public void SetPageNumber(string pageNumber)
        {
            pageText.text = pageNumber;
        }
        public void SetPageButtonInteractable(bool prev, bool next)
        {
            prevPageButton.interactable = prev;
            nextPageButton.interactable = next;
        }
        public void SetCharacter(TalkCharacterViewData viewData)
        {
            character.UpdateCharacter(viewData);
        }
        public void SetBackground(Sprite sprite)
        {
            background.sprite = sprite;
        }
        public void ShowTalk(string message)
        {
            speechBubble.SetText(message);
            speechBubble.SetShowing(true);
        }
        public void HideTalk()
        {
            speechBubble.SetShowing(false);
        }
        private void Awake()
        {
            returnButton.onClick.AddListener(() => OnReturnClick?.Invoke());
            prevPageButton.onClick.AddListener(() => OnPageButtonClick?.Invoke(false));
            nextPageButton.onClick.AddListener(() => OnPageButtonClick?.Invoke(true));
        }
        private void OnPageProductClickCallback(StoreProductItem item)
        {
            OnProductClick?.Invoke(products.indexOf(item));
        }
        private void OnPageProductPointerEnterCallback(StoreProductItem item)
        {
            OnProductPointerEnter?.Invoke(products.indexOf(item));
        }
        private void OnPageProductPointerExitCallback(StoreProductItem item)
        {
            OnProductPointerExit?.Invoke(products.indexOf(item));
        }
        public event Action OnReturnClick;
        public event Action<bool> OnPageButtonClick;
        public event Action<int> OnProductClick;
        public event Action<int> OnProductPointerEnter;
        public event Action<int> OnProductPointerExit;
        [SerializeField]
        private GameObject storeUIRoot;
        [SerializeField]
        private SpriteRenderer background;
        [SerializeField]
        private TalkCharacterController character;
        [SerializeField]
        private SpeechBubble speechBubble;
        [SerializeField]
        private Button returnButton;
        [SerializeField]
        private Button prevPageButton;
        [SerializeField]
        private Button nextPageButton;
        [SerializeField]
        private TextMeshProUGUI pageText;
        [SerializeField]
        private MoneyPanel moneyPanel;
        [SerializeField]
        private ElementList products;
    }
}
