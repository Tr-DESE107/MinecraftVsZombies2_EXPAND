using System;
using System.Collections.Generic;
using MVZ2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Map
{
    public class MapUI : MonoBehaviour
    {
        public void SetButtonActive(ButtonType button, bool active)
        {
            if (buttonDict.TryGetValue(button, out var btn))
            {
                btn.gameObject.SetActive(active);
            }
        }
        public void SetHintText(string text)
        {
            hintText.text = text;
        }
        public void SetDragRootVisible(bool visible)
        {
            dragRoot.SetActive(visible);
        }
        public void SetOptionsDialogActive(bool active)
        {
            optionsDialog.gameObject.SetActive(active);
        }
        public void SetDragRootPosition(Vector2 screenPos)
        {
            dragRootTrans.position = mapCamera.ScreenToWorldPoint(screenPos);
            var localPos = dragRootTrans.localPosition;
            localPos.z = 0;
            dragRootTrans.localPosition = localPos;
        }
        public void SetDragArrowTargetPosition(Vector2 screenPos)
        {
            var worldPos = mapCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;
            var angle = Vector2.SignedAngle(Vector2.up, (Vector2)(worldPos - dragArrowRoot.position));
            dragArrowRoot.eulerAngles = Vector3.forward * angle;

            var localPos = (Vector2)dragArrowRoot.parent.InverseTransformPoint(worldPos);
            dragArrowRoot.sizeDelta = new Vector2(0, localPos.magnitude);
        }
        private void Awake()
        {
            buttonDict.Add(ButtonType.Back, backButton);
            buttonDict.Add(ButtonType.Almanac, almanacButton);
            buttonDict.Add(ButtonType.Store, storeButton);
            buttonDict.Add(ButtonType.Setting, settingButton);

            foreach (var pair in buttonDict)
            {
                var button = pair.Key;
                pair.Value.onClick.AddListener(() => OnButtonClick?.Invoke(button));
            }
        }
        public event Action<ButtonType> OnButtonClick;

        private Dictionary<ButtonType, Button> buttonDict = new Dictionary<ButtonType, Button>();

        public OptionsDialog OptionsDialog => optionsDialog;

        [Header("General")]
        [SerializeField]
        private Button backButton;
        [SerializeField]
        private Button almanacButton;
        [SerializeField]
        private Button storeButton;
        [SerializeField]
        private Button settingButton;
        [SerializeField]
        private TextMeshProUGUI hintText;
        [SerializeField]
        private OptionsDialog optionsDialog;

        [Header("Drag")]
        [SerializeField]
        private Camera mapCamera;
        [SerializeField]
        private GameObject dragRoot;
        [SerializeField]
        private RectTransform dragRootTrans;
        [SerializeField]
        private RectTransform dragArrowRoot;

        public enum ButtonType
        {
            Back,
            Almanac,
            Store,
            Setting
        }
    }
}
