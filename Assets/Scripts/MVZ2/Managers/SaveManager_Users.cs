using System.IO;
using MVZ2.Save;
using MVZ2.Serialization;
using UnityEngine;

namespace MVZ2.Managers
{
    public partial class SaveManager : MonoBehaviour
    {
        #region 修改
        public void SetUserName(int index, string name)
        {
            var meta = userDataList.Get(index);
            if (meta == null)
            {
                meta = userDataList.Create(index);
            }
            meta.Username = name;
        }
        public void SetCurrentUserIndex(int index)
        {
            userDataList.CurrentUserIndex = index;
            ReloadCurrentUserData();
        }
        public int CreateNewUser(string name)
        {
            var index = FindFreeUserIndex();
            SetUserName(index, name);
            return index;
        }
        public void DeleteUser(int index)
        {
            userDataList.Delete(index);
            DeleteUserSaveData(index);
        }
        #endregion

        #region 获取
        public UserDataItem[] GetAllUsers()
        {
            return userDataList.GetAllUsers();
        }
        public int GetCurrentUserIndex()
        {
            return userDataList.CurrentUserIndex;
        }
        public string GetCurrentUserName()
        {
            var index = userDataList.CurrentUserIndex;
            return GetUserName(index);
        }
        public string GetUserName(int index)
        {
            var meta = userDataList.Get(index);
            if (meta == null)
                return null;
            return meta.Username;
        }
        public bool HasDuplicateUserName(string name, int indexToExcept)
        {
            for (int i = 0; i < userDataList.GetMaxUserCount(); i++)
            {
                if (i == indexToExcept)
                    continue;
                var meta = userDataList.Get(i);
                if (meta == null)
                    continue;
                if (meta.Username == name)
                    return true;
            }
            return false;
        }
        public bool UserExists(int i)
        {
            return userDataList.Get(i) != null;
        }
        public int FindFreeUserIndex()
        {
            for (int i = 0; i < MAX_USER_COUNT; i++)
            {
                if (!UserExists(i))
                    return i;
            }
            return -1;
        }
        #endregion

        #region 保存
        public void SaveUserList()
        {
            var saveDataMetaPath = GetUserListPath();
            FileHelper.ValidateDirectory(saveDataMetaPath);
            var serializable = userDataList.ToSerializable();
            var metaJson = serializable.ToBson();
            Main.FileManager.WriteJsonFile(saveDataMetaPath, metaJson);
        }
        #endregion

        #region 读取
        private void LoadUserList()
        {
            var saveDataMetaPath = GetUserListPath();
            if (!File.Exists(saveDataMetaPath))
            {
                userDataList = new UserDataList(MAX_USER_COUNT);
            }
            else
            {
                var metaJson = Main.FileManager.ReadJsonFile(saveDataMetaPath);
                var serializable = SerializeHelper.FromBson<SerializableUserDataList>(metaJson);
                userDataList = UserDataList.FromSerializable(serializable);
            }
        }
        #endregion

        #region 路径
        public string GetUserListPath()
        {
            return Path.Combine(GetSaveDataRoot(), "users.dat");
        }
        #endregion


        #region 属性字段
        public const int MAX_USER_COUNT = 8;
        private UserDataList userDataList;
        #endregion
    }
}
