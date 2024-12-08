using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Managers;
using MVZ2.Save;
using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.Mainmenu.UI
{
    public class DeleteUserDialogController : MonoBehaviour
    {
        public Task<int> Show(IEnumerable<UserDataItem> users)
        {
            ui.ResetPosition();
            gameObject.SetActive(true);
            if (tcs != null)
                return tcs.Task;

            var userIndexes = users.Select((user, index) => (user, index)).Where(a => a.user != null).Select(p => p.index);
            managingUserIndexes = userIndexes.ToArray();
            selectedUserArrayIndex = 0;
            ui.UpdateUsers(users.Select(u => u.Username).ToArray());

            tcs = new TaskCompletionSource<int>();
            return tcs.Task;
        }
        private void Hide()
        {
            gameObject.SetActive(false);
            selectedUserArrayIndex = -1;
            if (tcs == null)
                return;
            tcs.TrySetResult(-1);
            tcs = null;
        }
        private void Awake()
        {
            ui.OnUserSelect += OnUserSelectCallback;
            ui.OnDeleteButtonClick += OnDeleteButtonClickCallback;
        }
        private void OnUserSelectCallback(int index)
        {
            selectedUserArrayIndex = index;
            ui.SetDeleteButtonInteractable(index >= 0);
        }
        private async void OnDeleteButtonClickCallback()
        {
            var userIndex = GetSelectedUserIndex();
            var title = main.LanguageManager._(VanillaStrings.WARNING);
            var desc = main.LanguageManager._(VanillaStrings.WARNING_DELETE_USER, main.SaveManager.GetUserName(userIndex));
            var result = await main.Scene.ShowDialogSelectAsync(title, desc);
            if (result)
            {
                main.SaveManager.DeleteUser(userIndex);
                main.SaveManager.SaveUserList();
                tcs.SetResult(userIndex);
                Hide();
            }
        }
        private int GetSelectedUserIndex()
        {
            return managingUserIndexes[selectedUserArrayIndex];
        }
        private MainManager main => MainManager.Instance;
        [SerializeField]
        private DeleteUserDialog ui;
        private int selectedUserArrayIndex;
        private int[] managingUserIndexes;
        private TaskCompletionSource<int> tcs;
    }
}
