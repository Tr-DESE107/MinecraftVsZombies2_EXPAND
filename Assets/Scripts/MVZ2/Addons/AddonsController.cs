using System;
using MVZ2.Scenes;
using UnityEngine;

namespace MVZ2.Addons
{
    public class AddonsController : MainScenePage
    {
        public override void Display()
        {
            base.Display();
            DisplayIndex();
            languagePacks.Hide();
        }
        public void DisplayIndex()
        {
            ui.SetIndexVisible(true);
        }
        public void SetLoadingVisible(bool visible)
        {
            ui.SetLoadingVisible(visible);
        }
        private void Awake()
        {
            ui.OnButtonClick += OnButtonClickCallback;
        }
        private async void OnButtonClickCallback(AddonsUI.Buttons button)
        {
            switch (button)
            {
                case AddonsUI.Buttons.Return:
                    Hide();
                    OnReturnClick?.Invoke();
                    break;
                case AddonsUI.Buttons.LanguagePack:
                    ui.SetIndexVisible(false);
                    SetLoadingVisible(true);
                    await languagePacks.Display();
                    SetLoadingVisible(false);
                    break;
            }
        }
        public event Action OnReturnClick;
        [SerializeField]
        private AddonsUI ui;
        [SerializeField]
        private LanguagePacksController languagePacks;
    }
}
