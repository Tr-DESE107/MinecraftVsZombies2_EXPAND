using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVZ2.Scenes;
using UnityEngine;

namespace MVZ2.Addons
{
    public class AddonsController : MainScenePage
    {
        public void DisplayIndex()
        {
            ui.SetIndexVisible(true);
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
                    await languagePacks.Display();
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
