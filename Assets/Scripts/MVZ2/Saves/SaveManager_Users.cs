using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Maps;
using MVZ2.GameContent.Stages;
using MVZ2.IO;
using MVZ2.Modding;
using MVZ2.OldSaves;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.Stats;
using MVZ2Logic;
using MVZ2Logic.Saves;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Saves
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
            LoadUserData(index);
            userDataList.CurrentUserIndex = index;
        }
        public int CreateNewUser(string name)
        {
            var index = FindFreeUserIndex();
            CreateNewUser(name, index);
            return index;
        }
        public void CreateNewUser(string name, int index)
        {
            SetUserName(index, name);
            var directory = GetUserSaveDataDirectory(index);
            if (Directory.Exists(directory))
            {
                Directory.Delete(directory, true);
            }
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
        public bool CanRenameUser(string userName)
        {
            return !Main.Game.IsSpecialUserName(userName);
        }
        public bool CanRenameUserTo(string userName)
        {
            return !Main.Game.IsSpecialUserName(userName);
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
                try
                {
                    var metaJson = Main.FileManager.ReadJsonFile(saveDataMetaPath);
                    var serializable = SerializeHelper.FromBson<SerializableUserDataList>(metaJson);
                    userDataList = UserDataList.FromSerializable(serializable);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error loading user list: {e}");
                    userDataList = new UserDataList(MAX_USER_COUNT);
                }
            }
        }
        #endregion

        #region 路径
        public string GetUserListPath()
        {
            return Path.Combine(GetSaveDataRoot(), "users.dat");
        }
        #endregion

        #region 导出
        public bool ExportUserDataPack(int userIndex, string destPath)
        {
            var userDir = GetUserSaveDataDirectory(userIndex);
            if (!Directory.Exists(userDir))
                return false;

            FileHelper.ValidateDirectory(destPath);
            var sourceDirInfo = new DirectoryInfo(userDir);
            var files = Directory.GetFiles(userDir, "*", SearchOption.AllDirectories);
            using var stream = File.Open(destPath, FileMode.Create);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            foreach (var filePath in files)
            {
                var entryName = Path.Combine("userdata", Path.GetRelativePath(userDir, filePath));
                entryName = entryName.Replace("\\", "/");
                archive.CreateEntryFromFile(filePath, entryName);
            }
            var userName = GetUserName(userIndex);
            var metadata = new UserDataPackMetadata(userName);
            var seri = new SerializableUserDataPackMetadata(metadata);
            var json = SerializeHelper.ToBson(seri);

            var metadataEntry = archive.CreateEntry(USER_DATA_PACK_METADATA_ENTRY_NAME);
            metadataEntry.WriteString(json, Encoding.UTF8);
            return true;
        }
        #endregion

        #region 导入
        public void ImportUserDataPack(string userName, int userIndex, string sourcePath)
        {
            if (!File.Exists(sourcePath))
                return;
            if (userIndex < 0)
                return;
            var userDir = GetUserSaveDataDirectory(userIndex);
            if (Directory.Exists(userDir))
            {
                Directory.Delete(userDir, true);
            }

            using var stream = File.Open(sourcePath, FileMode.Open);
            using var archive = new ZipArchive(stream);

            var entries = archive.Entries.ToArray();
            foreach (var entry in entries)
            {
                if (string.IsNullOrEmpty(entry.Name))
                    continue;

                if (string.IsNullOrEmpty(Path.GetExtension(entry.FullName)))
                    continue;

                var fullPath = entry.FullName.Replace("\\", "/");
                var splitedPaths = fullPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                if (splitedPaths.Length > 1 && splitedPaths[0] == "userdata")
                {
                    var relativePath = Path.Combine(splitedPaths.Skip(1).ToArray());
                    var destPath = Path.Combine(userDir, relativePath);
                    FileHelper.ValidateDirectory(destPath);

                    var bytes = entry.ReadBytes();
                    using (var entryStream = File.Open(destPath, FileMode.CreateNew))
                    {
                        entryStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            SetUserName(userIndex, userName);
            SaveUserList();
            LoadUserList();
        }
        public UserDataPackMetadata ImportUserDataPackMetadata(string sourcePath)
        {
            if (!File.Exists(sourcePath))
                return null;
            using var stream = File.Open(sourcePath, FileMode.Open);
            using var archive = new ZipArchive(stream);

            var entry = archive.GetEntry(USER_DATA_PACK_METADATA_ENTRY_NAME);
            if (entry == null)
                return null;
            var json = entry.ReadString(Encoding.UTF8);
            var seri = SerializeHelper.FromBson<SerializableUserDataPackMetadata>(json);
            return UserDataPackMetadata.FromSerializable(seri);
        }
        #endregion

        #region 导入旧存档
        private UserDataList ImportOldUserList(OldUserList userList)
        {
            if (userList?.usernames == null)
                return null;

            // 用户列表
            var newUserDataList = new UserDataList(MAX_USER_COUNT);
            newUserDataList.CurrentUserIndex = -1;
            for (int i = 0; i < userList.usernames.Length; i++)
            {
                var name = userList.usernames[i];
                if (string.IsNullOrEmpty(name))
                    continue;
                var userItem = newUserDataList.Create(i);
                userItem.Username = name;
                if (newUserDataList.CurrentUserIndex < 0)
                {
                    newUserDataList.CurrentUserIndex = i;
                }
            }
            if (newUserDataList.CurrentUserIndex < 0)
            {
                newUserDataList.CurrentUserIndex = 0;
            }
            return newUserDataList;
        }
        private VanillaSaveData[] ImportOldUserData(UserDataList userList, OldSaveData[] userDatas)
        {
            if (userDatas == null)
                return null;

            ModInfo vanillaMod = null;
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                if (mod == null || mod.Logic == null)
                    continue;
                if (mod.Namespace != Main.BuiltinNamespace)
                    continue;
                vanillaMod = mod;
                break;
            }
            if (vanillaMod == null)
                return null;

            var allUsers = userList.GetAllUsers();
            VanillaSaveData[] results = new VanillaSaveData[allUsers.Length];
            for (int i = 0; i < allUsers.Length; i++)
            {
                var user = allUsers[i];
                if (user == null)
                    continue;
                var oldData = userDatas[i];
                if (oldData == null)
                    continue;
                ModSaveData data = vanillaMod.Logic.CreateSaveData();
                if (data is not VanillaSaveData vanilla)
                    continue;
                ImportUserDataFromOld(vanilla, oldData);
                results[i] = vanilla;
            }
            return results;
        }
        private void ImportUserDataFromOld(VanillaSaveData saveData, OldSaveData oldData)
        {
            if (oldData == null)
                return;
            ImportMainUserDataFromOld(saveData, oldData.main);
            ImportAchievementsUserDataFromOld(saveData, oldData.achievements);
            ImportEndlessUserDataFromOld(saveData, oldData.endless);
        }
        private void ImportMainUserDataFromOld(VanillaSaveData saveData, OldSaveDataMain oldData)
        {
            if (oldData == null)
                return;

            // 关卡进度。
            for (int i = 0; i < oldLevelIDList.Length; i++)
            {
                if (oldData.currentLevel > i)
                {
                    var unlockName = VanillaSaveExt.GetLevelClearUnlockID(oldLevelIDList[i]);
                    saveData.Unlock(unlockName);
                }
            }
            if ((oldData.currentLevel == 12 && oldData.sidePlot == 1) || oldData.currentLevel > 12)
            {
                saveData.Unlock(VanillaUnlockNames.enteredDream);
            }
            // 关卡难度。
            for (int i = 0; i < oldData.levelDifficulties.Length; i++)
            {
                var difficulty = oldData.levelDifficulties[i];
                var index = i + 1;
                if (index >= oldLevelIDList.Length || index > oldData.currentLevel)
                    continue;
                var stageID = oldLevelIDList[index];
                NamespaceID diff = VanillaDifficulties.hard;
                switch (difficulty)
                {
                    case 0:
                        diff = VanillaDifficulties.easy;
                        break;
                    case 1:
                        diff = VanillaDifficulties.normal;
                        break;
                }
                saveData.AddLevelDifficultyRecord(stageID, diff);
            }
            // 上一张地图
            if (oldData.lastMap == "mvz2:dream_map")
            {
                saveData.LastMapID = VanillaMapID.dream;
            }
            else
            {
                saveData.LastMapID = VanillaMapID.halloween;
            }
            // 金钱
            saveData.SetMoney(oldData.money);
            // 卡槽
            if (oldData.cardSlots >= 7)
                saveData.Unlock(VanillaUnlockNames.blueprintSlot1);
            if (oldData.cardSlots >= 8)
                saveData.Unlock(VanillaUnlockNames.blueprintSlot2);
            if (oldData.cardSlots >= 9)
                saveData.Unlock(VanillaUnlockNames.blueprintSlot3);
            if (oldData.cardSlots >= 10)
                saveData.Unlock(VanillaUnlockNames.blueprintSlot4);
            // 星之碎片槽
            if (oldData.starshardSlots >= 4)
                saveData.Unlock(VanillaUnlockNames.starshardSlot1);
            if (oldData.starshardSlots >= 5)
                saveData.Unlock(VanillaUnlockNames.starshardSlot2);
            // 升级
            if ((oldData.upgrades & 1) != 0)
            {
                saveData.Unlock(VanillaUnlockNames.infectenser);
            }
            if ((oldData.upgrades & 2) != 0)
            {
                saveData.Unlock(VanillaUnlockNames.forcePad);
            }
            // 梦魇
            if (oldData.nightmare)
            {
                saveData.Unlock(VanillaUnlockNames.dreamIsNightmare);
            }
            // 教程
            if (oldData.moneyFound)
            {
                saveData.Unlock(VanillaUnlockNames.money);
            }
            if (oldData.starshardLearnt)
            {
                saveData.Unlock(VanillaUnlockNames.starshard);
            }
            if (oldData.triggerLearnt)
            {
                saveData.Unlock(VanillaUnlockNames.trigger);
            }
        }
        private void ImportAchievementsUserDataFromOld(VanillaSaveData saveData, OldSaveDataAchievements oldData)
        {
            if (oldData == null)
                return;
            if (oldData.earned == null || oldData.earned.Length < 1)
                return;
            var earned = oldData.earned[0];
            for (int i = 0; i < oldAchievementIDList.Length; i++)
            {
                if ((earned & (1 << i)) != 0)
                {
                    saveData.Unlock(oldAchievementIDList[i]);
                }
            }
        }
        private void ImportEndlessUserDataFromOld(VanillaSaveData saveData, OldSaveDataEndless oldData)
        {
            if (oldData == null || oldData.flags == null)
                return;
            foreach (var pair in oldData.flags)
            {
                if (pair.Value == null)
                    continue;
                var stageID = VanillaStageNames.halloweenEndless;
                switch (pair.Key)
                {
                    case "dream":
                        stageID = VanillaStageNames.dreamEndless;
                        break;
                }
                saveData.SetCurrentEndlessFlag(stageID, pair.Value.current);
                saveData.SetStat(VanillaStats.CATEGORY_MAX_ENDLESS_FLAGS.Path, new NamespaceID(Main.BuiltinNamespace, stageID), pair.Value.max);
            }
        }
        private void ImportOldSaveData(OldGlobalSave saveData)
        {
            if (saveData == null)
                return;

            // 用户列表
            var newUserDataList = ImportOldUserList(saveData.userList);
            if (newUserDataList == null)
                return;

            var saveDataMetaPath = GetUserListPath();
            FileHelper.ValidateDirectory(saveDataMetaPath);
            var seriUserList = newUserDataList.ToSerializable();
            var userListJson = seriUserList.ToBson();
            Main.FileManager.WriteJsonFile(saveDataMetaPath, userListJson);

            // 存档。
            var saveDatas = ImportOldUserData(newUserDataList, saveData.saveDatas);
            if (saveDatas == null)
                return;

            var spaceName = Main.BuiltinNamespace;
            for (int i = 0; i < saveDatas.Length; i++)
            {
                var modSaveData = saveDatas[i];
                if (modSaveData == null)
                    continue;
                var path = GetUserModSaveDataPath(i, spaceName);
                FileHelper.ValidateDirectory(path);
                var seriUserData = modSaveData.ToSerializable();
                var userDataJson = seriUserData.ToBson();
                Main.FileManager.WriteJsonFile(path, userDataJson);
            }
        }
        #endregion

        #region 属性字段
        public const string USER_DATA_PACK_METADATA_ENTRY_NAME = "metadata.json";
        public const int MAX_USER_COUNT = 8;
        private static readonly string[] oldLevelIDList = new string[]
        {
            VanillaStageNames.prologue, // 0

            VanillaStageNames.halloween1,
            VanillaStageNames.halloween2,
            VanillaStageNames.halloween3,
            VanillaStageNames.halloween4,
            VanillaStageNames.halloween5,
            VanillaStageNames.halloween6,
            VanillaStageNames.halloween7,
            VanillaStageNames.halloween8,
            VanillaStageNames.halloween9,
            VanillaStageNames.halloween10,
            VanillaStageNames.halloween11, // 11

            VanillaStageNames.dream1,
            VanillaStageNames.dream2,
            VanillaStageNames.dream3,
            VanillaStageNames.dream4,
            VanillaStageNames.dream5,
            VanillaStageNames.dream6,
            VanillaStageNames.dream7,
            VanillaStageNames.dream8,
            VanillaStageNames.dream9,
            VanillaStageNames.dream10,
            VanillaStageNames.dream11, // 22
        };
        private static readonly string[] oldAchievementIDList = new string[]
        {
            VanillaUnlockNames.doubleTrouble,
            VanillaUnlockNames.ghostBuster,
            VanillaUnlockNames.rickrollDrown,
            VanillaUnlockNames.returnToSender,
        };
        private UserDataList userDataList;
        #endregion
    }
}
