using System;
using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Saves;
using MVZ2.Scenes;
using MVZ2.Talk;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2Logic;
using PVZEngine;
using Tools;
using UnityEngine;

namespace MVZ2.Store
{
    public class StoreController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            UpdateProducts();
            page = 0;
            UpdatePage();
            ResetChatTimeout();
            chatFadeTimeout = 0;
            ui.Display();

            var presets = Main.ResourceManager.GetAllStorePresets();
            var filteredPresets = presets.Where(p => p.Conditions == null || Main.SaveManager.MeetsXMLConditions(p.Conditions));
            var preset = filteredPresets.OrderByDescending(p => p.Priority).FirstOrDefault();
            SetPreset(preset);
        }
        public void SetPreset(StorePresetMeta preset)
        {
            characterId = preset.Character;
            var backgroundSprite = Main.GetFinalSprite(preset.Background);

            var viewData = Main.ResourceManager.GetCharacterViewData(characterId, null, CharacterSide.Left);
            ui.SetCharacter(viewData);
            ui.SetBackground(backgroundSprite);
            if (!Main.MusicManager.IsPlaying(preset.Music))
                Main.MusicManager.Play(preset.Music);
        }
        private void Awake()
        {
            ui.OnReturnClick += OnReturnClickCallback;
            ui.OnPageButtonClick += OnPageButtonClickCallback;
            ui.OnProductPointerEnter += OnProductPointerEnterCallback;
            ui.OnProductPointerExit += OnProductPointerExitCallback;
            ui.OnProductClick += OnProductClickCallback;

            chatRNG = new RandomGenerator(new Guid().GetHashCode());
        }
        private void Update()
        {
            if (!pointingProduct)
            {
                chatTimeout -= Time.deltaTime;
                if (chatTimeout <= 0)
                {
                    ShowChat();
                    ResetChatTimeout();
                    chatFadeTimeout = MAX_CHAT_FADE_TIMEOUT;
                }
                chatFadeTimeout -= Time.deltaTime;
                if (chatFadeTimeout <= 0)
                {
                    ui.HideTalk();
                }
            }
        }
        private void OnReturnClickCallback()
        {
            Hide();
            OnReturnClick?.Invoke();
        }
        private void OnPageButtonClickCallback(bool next)
        {
            var offset = next ? 1 : -1;
            page += offset;
            var totalPages = GetTotalPages();
            if (page < 0)
            {
                page = totalPages - 1;
            }
            else if (page >= totalPages)
            {
                page = 0;
            }
            UpdatePage();
        }
        private void OnProductPointerEnterCallback(int index)
        {
            var product = GetCurrentProduct(index);
            if (!NamespaceID.IsValid(product))
                return;
            var productMeta = Main.ResourceManager.GetProductMeta(product);
            var textKey = productMeta.GetMessage(characterId);
            var message = GetTranslatedString(VanillaStrings.CONTEXT_STORE_TALK, textKey);
            pointingProduct = true;
            ui.ShowTalk(message);
        }
        private void OnProductPointerExitCallback(int index)
        {
            pointingProduct = false;
            ui.HideTalk();
        }
        private void OnProductClickCallback(int index)
        {

        }
        private void UpdateProducts()
        {
            products.Clear();
            var productEntries = Main.SaveManager.GetUnlockedProducts();
            Main.StoreManager.GetOrderedProducts(productEntries, productsPerRow, products);
        }
        private void UpdatePage()
        {
            var viewDatas = products.Skip(page * productsPerPage).Take(productsPerPage).Select(c => Main.StoreManager.GetProductViewData(c)).ToArray();
            ui.SetProducts(viewDatas);

            var totalPages = GetTotalPages();
            bool interactable = totalPages > 1;
            ui.SetPageButtonInteractable(interactable, interactable);

            ui.SetPageNumber(Main.LanguageManager._(PAGE_TEMPLATE, page + 1, totalPages));
        }
        private int GetTotalPages()
        {
            return Mathf.CeilToInt(products.Count / (float)productsPerPage);
        }
        private void ShowChat()
        {
            var chat = Main.StoreManager.GetRandomChat(characterId, chatRNG);
            if (chat == null)
                return;
            var message = GetTranslatedString(VanillaStrings.CONTEXT_STORE_TALK, chat.Text);
            ui.ShowTalk(message);
            Main.SoundManager.Play2D(chat.Sound);
        }
        private NamespaceID GetCurrentProduct(int index)
        {
            var i = page * productsPerPage + index;
            if (i < 0 || i >= products.Count)
                return null;
            return products[i];
        }
        private void ResetChatTimeout()
        {
            chatTimeout = MAX_CHAT_TIMEOUT;
        }
        private string GetTranslatedString(string context, string text, params object[] args)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;
            return Main.LanguageManager._p(context, text, args);
        }
        public event Action OnReturnClick;

        [TranslateMsg("商店的页面计数，{0}为当前页面，{1}为总页面")]
        public const string PAGE_TEMPLATE = "{0}/{1}";
        private MainManager Main => MainManager.Instance;
        private List<NamespaceID> products = new List<NamespaceID>();
        private const float MAX_CHAT_TIMEOUT = 10;
        private const float MAX_CHAT_FADE_TIMEOUT = 5;
        private float chatTimeout;
        private float chatFadeTimeout;
        private NamespaceID characterId;
        private bool pointingProduct;
        private int page;
        private RandomGenerator chatRNG;
        [SerializeField]
        private StoreUI ui;
        [SerializeField]
        private int productsPerRow = 4;
        [SerializeField]
        private int productsPerPage = 8;
    }
}
