using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.UI
{
    public class UserManageDialog : Dialog
    {
        public void UpdateUsers(string[] names)
        {
            userList.updateList(names.Length, (i, rect) =>
            {
                var item = rect.GetComponent<UserManageItem>();
                item.SetName(names[i]);
            },
            rect =>
            {
                var item = rect.GetComponent<UserManageItem>();
                item.OnValueChanged += OnItemValueChangedCallback;
            },
            rect =>
            {
                var item = rect.GetComponent<UserManageItem>();
                item.OnValueChanged -= OnItemValueChangedCallback;
            });
        }
        public void SelectUser(int index)
        {
            foreach (var user in userList.getElements<UserManageItem>())
            {
                var i = userList.indexOf(user);
                user.SetIsOn(i == index);
            }
        }
        public void SetButtonInteractable(ButtonType type, bool interactable)
        {
            if (buttonDict.TryGetValue(type, out var button))
            {
                button.interactable = interactable;
            }
        }
        public void SetCreateNewUserActive(bool active)
        {
            createNewUserButton.gameObject.SetActive(active);
        } 
        private void Awake()
        {
            buttonDict.Add(ButtonType.CreateNewUser, createNewUserButton);
            buttonDict.Add(ButtonType.Rename, renameButton);
            buttonDict.Add(ButtonType.Delete, deleteButton);
            buttonDict.Add(ButtonType.Switch, switchButton);
            buttonDict.Add(ButtonType.Back, backButton);


            foreach (var pair in buttonDict)
            {
                var type = pair.Key;
                pair.Value.onClick.AddListener(() => OnButtonClick?.Invoke(type));
            }
        }
        private void OnItemValueChangedCallback(UserManageItem item, bool value)
        {
            if (value)
                OnUserSelect?.Invoke(userList.indexOf(item));
        }
        public event Action<int> OnUserSelect;
        public event Action<ButtonType> OnButtonClick;

        private Dictionary<ButtonType, Button> buttonDict = new Dictionary<ButtonType, Button>();

        [SerializeField]
        private ElementList userList;
        [SerializeField]
        private Button createNewUserButton;
        [SerializeField]
        private Button renameButton;
        [SerializeField]
        private Button deleteButton;
        [SerializeField]
        private Button switchButton;
        [SerializeField]
        private Button backButton;
        public enum ButtonType
        {
            CreateNewUser,
            Rename,
            Delete,
            Switch,
            Back
        }
    }
}
