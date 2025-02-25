using System;
using System.Collections.Generic;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Addons
{
    public class LanguagePacksUI : MonoBehaviour
    {
        public void DeselectAll()
        {
            toggleGroup.SetAllTogglesOff();
        }
        public void SetDisabledLanguagePacks(LanguagePackViewData[] viewDatas)
        {
            disabledLanguagePacks.updateList(viewDatas.Length, (i, obj) =>
            {
                var packItem = obj.GetComponent<LanguagePackItem>();
                packItem.UpdateItem(viewDatas[i]);
            },
            obj =>
            {
                var packItem = obj.GetComponent<LanguagePackItem>();
                packItem.OnToggled += OnDisabledPackItemToggledCallback;
            },
            obj =>
            {
                var packItem = obj.GetComponent<LanguagePackItem>();
                packItem.OnToggled -= OnDisabledPackItemToggledCallback;
            });
        }
        public void SetEnabledLanguagePacks(LanguagePackViewData[] viewDatas)
        {
            enabledLanguagePacks.updateList(viewDatas.Length, (i, obj) =>
            {
                var packItem = obj.GetComponent<LanguagePackItem>();
                packItem.UpdateItem(viewDatas[i]);
            },
            obj =>
            {
                var packItem = obj.GetComponent<LanguagePackItem>();
                packItem.OnToggled += OnEnabledPackItemToggledCallback;
            },
            obj =>
            {
                var packItem = obj.GetComponent<LanguagePackItem>();
                packItem.OnToggled -= OnEnabledPackItemToggledCallback;
            });
        }
        public void SetButtonInteractable(Buttons button, bool value)
        {
            if (buttonDict.TryGetValue(button, out var btn))
            {
                btn.interactable = value;
            }
        }
        private void Awake()
        {
            buttonDict.Add(Buttons.Disable, disableButton);
            buttonDict.Add(Buttons.Enable, enableButton);
            buttonDict.Add(Buttons.Import, importButton);
            buttonDict.Add(Buttons.Export, exportButton);
            buttonDict.Add(Buttons.Delete, deleteButton);
            buttonDict.Add(Buttons.Return, returnButton);

            foreach (var pair in buttonDict)
            {
                pair.Value.onClick.AddListener(() => OnButtonClick?.Invoke(pair.Key));
            }
        }
        private void OnDisabledPackItemToggledCallback(LanguagePackItem item, bool value)
        {
            OnPackItemToggled?.Invoke(false, disabledLanguagePacks.indexOf(item), value);
        }
        private void OnEnabledPackItemToggledCallback(LanguagePackItem item, bool value)
        {
            OnPackItemToggled?.Invoke(true, enabledLanguagePacks.indexOf(item), value);
        }
        public event Action<bool, int, bool> OnPackItemToggled;
        public event Action<Buttons> OnButtonClick;
        [SerializeField]
        private ToggleGroup toggleGroup;
        [SerializeField]
        private ElementList disabledLanguagePacks;
        [SerializeField]
        private ElementList enabledLanguagePacks;
        [SerializeField]
        private Button disableButton;
        [SerializeField]
        private Button enableButton;
        [SerializeField]
        private Button importButton;
        [SerializeField]
        private Button exportButton;
        [SerializeField]
        private Button deleteButton;
        [SerializeField]
        private Button returnButton;
        private Dictionary<Buttons, Button> buttonDict = new Dictionary<Buttons, Button>();

        public enum Buttons
        {
            Disable,
            Enable,
            Import,
            Export,
            Delete,
            Return
        }
    }
}
