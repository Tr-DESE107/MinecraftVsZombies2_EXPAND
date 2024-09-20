using System;
using TMPro;
using UnityEngine;

namespace MVZ2.UI
{
    public class CustomDialog : Dialog
    {
        public void SetDialog(string titleText, string descText, string[] options, Action<int> onSelect)
        {
            title.text = titleText;
            desc.text = descText;
            OnOptionSelect = onSelect;
            buttonList.updateList(options.Length, (i, rect) =>
            {
                var button = rect.GetComponent<TextButton>();
                button.Text.text = options[i];
            },
            rect =>
            {
                var button = rect.GetComponent<TextButton>();
                button.Button.onClick.RemoveAllListeners();
                button.Button.onClick.AddListener(() => OnOptionClickCallback(buttonList.indexOf(rect)));
            });
        }
        private void OnOptionClickCallback(int index)
        {
            OnOptionSelect?.Invoke(index);
        }
        private Action<int> OnOptionSelect;
        [SerializeField]
        private TextMeshProUGUI title;
        [SerializeField]
        private TextMeshProUGUI desc;
        [SerializeField]
        private ElementList buttonList;

    }
}
