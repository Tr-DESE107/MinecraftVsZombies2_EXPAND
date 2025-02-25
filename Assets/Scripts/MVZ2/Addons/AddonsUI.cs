using System;
using System.Collections.Generic;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Addons
{
    public class AddonsUI : MonoBehaviour
    {
        public void SetIndexVisible(bool visible)
        {
            indexUI.SetActive(visible);
        }
        public void SetLoadingVisible(bool visible)
        {
            loadingScreen.SetActive(visible);
        }
        private void Awake()
        {
            buttonDict.Add(Buttons.LanguagePack, languagePackButton);
            buttonDict.Add(Buttons.Return, returnButton);

            foreach (var pair in buttonDict)
            {
                pair.Value.onClick.AddListener(() => OnButtonClick?.Invoke(pair.Key));
            }
        }
        public event Action<bool, int, bool> OnPackItemToggled;
        public event Action<Buttons> OnButtonClick;
        [SerializeField]
        private Button languagePackButton;
        [SerializeField]
        private Button returnButton;
        [SerializeField]
        private GameObject indexUI;
        [SerializeField]
        private GameObject loadingScreen;
        private Dictionary<Buttons, Button> buttonDict = new Dictionary<Buttons, Button>();

        public enum Buttons
        {
            LanguagePack,
            Return
        }
    }
}
