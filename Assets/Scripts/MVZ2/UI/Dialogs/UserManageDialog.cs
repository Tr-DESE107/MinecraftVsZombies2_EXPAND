using System;
using System.Collections.Generic;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu.UI
{
    public class UserManageDialog : Dialog
    {
        public void UpdateUsers(UserNameItemViewData[] names)
        {
            userList.UpdateUsers(names);
        }
        public void SelectUser(int index)
        {
            userList.SelectUser(index);
        }
        public void SetCreateNewUserActive(bool active)
        {
            userList.SetCreateNewUserActive(active);
        }
        public void SetButtonInteractable(ButtonType type, bool interactable)
        {
            if (buttonDict.TryGetValue(type, out var button))
            {
                button.interactable = interactable;
            }
        }
        private void Awake()
        {
            buttonDict.Add(ButtonType.Rename, renameButton);
            buttonDict.Add(ButtonType.Delete, deleteButton);
            buttonDict.Add(ButtonType.Switch, switchButton);
            buttonDict.Add(ButtonType.Back, backButton);

            foreach (var pair in buttonDict)
            {
                var type = pair.Key;
                pair.Value.onClick.AddListener(() => OnButtonClick?.Invoke(type));
            }

            userList.OnUserSelect += (index) => OnUserSelect?.Invoke(index);
            userList.OnCreateNewUserButtonClick += () => OnCreateNewUserButtonClick?.Invoke();
        }
        public event Action<int> OnUserSelect;
        public event Action OnCreateNewUserButtonClick;
        public event Action<ButtonType> OnButtonClick;

        private Dictionary<ButtonType, Button> buttonDict = new Dictionary<ButtonType, Button>();

        [SerializeField]
        private UserManageList userList;
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
            Rename,
            Delete,
            Switch,
            Back
        }
    }
}
