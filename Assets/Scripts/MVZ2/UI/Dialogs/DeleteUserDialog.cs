using System;
using MVZ2.UI;
using UnityEngine;
using UnityEngine.UI;

namespace MVZ2.Mainmenu.UI
{
    public class DeleteUserDialog : Dialog
    {
        public void UpdateUsers(string[] names)
        {
            userList.UpdateUsers(names);
        }
        public void SelectUser(int index)
        {
            userList.SelectUser(index);
        }
        public void SetDeleteButtonInteractable(bool interactable)
        {
            deleteButton.interactable = interactable;
        }
        private void Awake()
        {
            deleteButton.onClick.AddListener(() => OnDeleteButtonClick?.Invoke());
            userList.OnUserSelect += (index) => OnUserSelect?.Invoke(index);
            userList.SetCreateNewUserActive(false);
        }
        public event Action<int> OnUserSelect;
        public event Action OnDeleteButtonClick;

        [SerializeField]
        private UserManageList userList;
        [SerializeField]
        private Button deleteButton;
    }
}
