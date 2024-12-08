using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVZ2.IO;
using MVZ2.Managers;
using MVZ2Logic;
using MVZ2Logic.Games;
using MVZ2Logic.Saves;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;

namespace MVZ2.Saves
{
    public partial class SaveManager : MonoBehaviour, IGameSaveData, IGlobalSave
    {
        #region 保存
        public void SaveModDatas()
        {
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                SaveCurrentModData(mod.Namespace);
            }
        }
        public void SaveCurrentModData(string spaceName)
        {
            if (userDataList == null)
                return;
            SaveModData(userDataList.CurrentUserIndex, spaceName);
        }
        public void SaveModData(int userIndex, string spaceName)
        {
            var path = GetUserModSaveDataPath(userIndex, spaceName);
            FileHelper.ValidateDirectory(path);
            var modSaveData = GetModSaveData(spaceName);
            if (modSaveData == null)
                return;
            var serializable = modSaveData.ToSerializable();
            var metaJson = serializable.ToBson();
            Main.FileManager.WriteJsonFile(path, metaJson);
        }
        #endregion

        #region 加载
        public void Load()
        {
            LoadUserList();
            LoadInitialUserData();
        }
        public void LoadUserData(int index)
        {
            modSaveDatas.Clear();
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                LoadModData(index, mod.Namespace);
            }
            EvaluateUnlocks(true);
        }
        public SaveDataStatus GetSaveDataStatus()
        {
            return status;
        }
        private void LoadInitialUserData()
        {
            // 预备一个所有存档的列表。
            var userIndexes = userDataList.GetAllUsers().Where(u => u != null).Select((u, i) => i).ToList();
            // 将当前存档索引调至第一位。
            userIndexes.Remove(userDataList.CurrentUserIndex);
            userIndexes.Insert(0, userDataList.CurrentUserIndex);

            // 按照顺序加载所有存档。
            foreach (var index in userIndexes)
            {
                try
                {
                    // 加载用户存档。
                    LoadUserData(index);
                    // 加载存档成功，将当前用户索引设置为该存档的索引，然后跳出。
                    userDataList.CurrentUserIndex = index;
                    SaveUserList();
                    return;
                }
                catch (Exception e)
                {
                    // 加载存档失败，弹出警告。
                    status.AddCorruptedUserIndex(index);
                    status.State = SaveDataState.SomeCorrupted;
                    Debug.LogWarning($"存档加载失败：{e}");
                }
            }
            // 没有存档有效。
            if (userIndexes.Count < MAX_USER_COUNT)
            {
                // 尝试创建一个存档。
                status.State = SaveDataState.AllCorrupted;
            }
            else
            {
                // 如果存档栏位都满了，强制要求玩家删除一个。
                status.State = SaveDataState.FullCorrupted;
            }
        }
        private void LoadModData(int userIndex, string spaceName)
        {
            var path = GetUserModSaveDataPath(userIndex, spaceName);
            var modInfo = Main.ModManager.GetModInfo(spaceName);
            if (modInfo == null || modInfo.Logic == null)
                return;
            ModSaveData saveData;
            if (!File.Exists(path))
            {
                saveData = modInfo.Logic.CreateSaveData();
            }
            else
            {
                var saveDataJson = Main.FileManager.ReadJsonFile(path);
                saveData = modInfo.Logic.LoadSaveData(saveDataJson);
            }
            modSaveDatas.Add(saveData);
        }
        #endregion

        #region 获取
        public ModSaveData GetModSaveData(string spaceName)
        {
            return modSaveDatas.FirstOrDefault(s => s.Namespace == spaceName);
        }
        public T GetModSaveData<T>(string spaceName)
        {
            foreach (var saveData in modSaveDatas)
            {
                if (saveData.Namespace == spaceName && saveData is T tData)
                {
                    return tData;
                }
            }
            return default;
        }
        public bool IsUnlocked(NamespaceID unlockId)
        {
            if (unlockId == null)
                return false;
            var modSaveData = GetModSaveData(unlockId.spacename);
            if (modSaveData == null)
                return false;
            return modSaveData.IsUnlocked(unlockId.path);
        }
        public NamespaceID[] GetLevelDifficultyRecords(NamespaceID stageID)
        {
            if (stageID == null)
                return Array.Empty<NamespaceID>();
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return Array.Empty<NamespaceID>();
            return modSaveData.GetLevelDifficultyRecords(stageID.path);
        }
        public bool HasLevelDifficultyRecords(NamespaceID stageID, NamespaceID difficulty)
        {
            if (stageID == null || difficulty == null)
                return false;
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return false;
            return modSaveData.HasLevelDifficultyRecord(stageID.path, difficulty);
        }
        public NamespaceID[] GetUnlockedContraptions()
        {
            return unlockedContraptionsCache.ToArray();
        }
        public NamespaceID[] GetUnlockedEnemies()
        {
            return unlockedEnemiesCache.ToArray();
        }
        public NamespaceID[] GetUnlockedArtifacts()
        {
            return unlockedArtifactsCache.ToArray();
        }
        public NamespaceID GetMapPresetID(NamespaceID mapId)
        {
            if (mapId == null)
                return null;
            var modSaveData = GetModSaveData(mapId.spacename);
            if (modSaveData == null)
                return null;
            return modSaveData.GetMapPresetID(mapId.path);
        }

        #endregion

        #region 修改
        public void Unlock(NamespaceID unlockId)
        {
            if (unlockId == null)
                return;
            var modSaveData = GetModSaveData(unlockId.spacename);
            if (modSaveData == null)
                return;
            modSaveData.Unlock(unlockId.path);
            EvaluateUnlocks(false);
        }
        public void AddLevelDifficultyRecord(NamespaceID stageID, NamespaceID difficulty)
        {
            if (stageID == null || difficulty == null)
                return;
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return;
            modSaveData.AddLevelDifficultyRecord(stageID.path, difficulty);
        }
        public bool RemoveLevelDifficultyRecord(NamespaceID stageID, NamespaceID difficulty)
        {
            if (stageID == null || difficulty == null)
                return false;
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return false;
            return modSaveData.RemoveLevelDifficultyRecord(stageID.path, difficulty);
        }
        public bool DeleteUserSaveData(int index)
        {
            var path = GetUserSaveDataDirectory(index);
            if (!Directory.Exists(path))
                return false;
            Directory.Delete(path, true);
            return true;
        }
        public void SetMapPresetID(NamespaceID mapId, NamespaceID presetId)
        {
            if (mapId == null || presetId == null)
                return;
            var modSaveData = GetModSaveData(mapId.spacename);
            if (modSaveData == null)
                return;
            modSaveData.SetMapPresetID(mapId.path, presetId);
        }
        #endregion

        #region 路径
        public string GetSaveDataRoot()
        {
            return Path.Combine(Application.persistentDataPath, "userdata");
        }
        public string GetUserSaveDataDirectory(int userIndex)
        {
            return Path.Combine(GetSaveDataRoot(), $"user{userIndex}");
        }
        public string GetUserModSaveDataDirectory(int userIndex, string spaceName)
        {
            return Path.Combine(GetUserSaveDataDirectory(userIndex), spaceName);
        }
        public string GetUserModSaveDataPath(int userIndex, string spaceName)
        {
            return Path.Combine(GetUserModSaveDataDirectory(userIndex, spaceName), $"user.dat");
        }
        #endregion

        #region 统计
        public UserStats GetUserStats(string nsp)
        {
            var saveData = GetModSaveData(nsp);
            if (saveData == null)
                return null;
            return saveData.GetAllStats();
        }
        public long GetSaveStat(NamespaceID category, NamespaceID entry)
        {
            var saveData = GetModSaveData(category.spacename);
            if (saveData == null)
                return 0;
            return saveData.GetStat(category.path, entry);
        }
        public void AddSaveStat(NamespaceID category, NamespaceID entry, long value)
        {
            var saveData = GetModSaveData(category.spacename);
            if (saveData == null)
                return;
            saveData.AddStat(category.path, entry, value);
        }
        #endregion

        #region 成就
        public bool IsAchievementEarned(NamespaceID achievementID)
        {
            var resourceManager = Main.ResourceManager;
            var meta = resourceManager.GetAchievementMeta(achievementID);
            if (meta == null)
                return false;
            return !NamespaceID.IsValid(meta.Unlock) || IsUnlocked(meta.Unlock);
        }
        #endregion

        #region 私有方法
        private void EvaluateUnlocks(bool initial)
        {
            EvaluateUnlockedEntities();
            EvaluateUnlockedArtifacts();
            EvaluateUnlockedAchievements(initial);
        }
        private void EvaluateUnlockedArtifacts()
        {
            unlockedArtifactsCache.Clear();
            var resourceManager = Main.ResourceManager;
            var artifactsID = resourceManager.GetAllArtifactsID();
            foreach (var id in artifactsID)
            {
                var meta = resourceManager.GetArtifactMeta(id);
                if (meta == null)
                    continue;
                if (NamespaceID.IsValid(meta.Unlock) && !IsUnlocked(meta.Unlock))
                    continue;
                unlockedArtifactsCache.Add(id);
            }
        }
        private void EvaluateUnlockedEntities()
        {
            unlockedContraptionsCache.Clear();
            unlockedEnemiesCache.Clear();
            var resourceManager = Main.ResourceManager;
            var entitiesID = resourceManager.GetAllEntitiesID();
            foreach (var id in entitiesID)
            {
                var meta = resourceManager.GetEntityMeta(id);
                if (meta == null)
                    continue;
                if (NamespaceID.IsValid(meta.Unlock) && !IsUnlocked(meta.Unlock))
                    continue;
                if (meta.Type == EntityTypes.PLANT)
                {
                    unlockedContraptionsCache.Add(id);
                }
                else if (meta.Type == EntityTypes.ENEMY)
                {
                    unlockedEnemiesCache.Add(id);
                }
            }
        }
        private void EvaluateUnlockedAchievements(bool initial)
        {
            var resourceManager = Main.ResourceManager;
            var achievementsID = resourceManager.GetAllAchievements();
            var newAchievements = achievementsID.Where(id => IsAchievementEarned(id));
            if (!initial)
            {
                Main.Scene.ShowAchievementEarnTips(newAchievements.Where(id => !unlockedAchievementsCache.Contains(id)));
            }
            unlockedAchievementsCache.Clear();
            unlockedAchievementsCache.AddRange(newAchievements);
        }
        #endregion

        #region 属性字段
        public MainManager Main => MainManager.Instance;
        private SaveDataStatus status = new SaveDataStatus();
        private List<ModSaveData> modSaveDatas = new List<ModSaveData>();
        private List<NamespaceID> unlockedContraptionsCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedEnemiesCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedArtifactsCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedAchievementsCache = new List<NamespaceID>();
        #endregion
    }
    public class SaveDataStatus
    {
        public SaveDataState State { get; set; } = SaveDataState.Success;
        public void AddCorruptedUserIndex(int index)
        {
            corruptedIndexes.Add(index);
        }
        public int[] GetCorruptedUserIndexes()
        {
            return corruptedIndexes.ToArray();
        }
        private List<int> corruptedIndexes = new List<int>();
    }
    public enum SaveDataState
    {
        Success,
        SomeCorrupted,
        AllCorrupted,
        FullCorrupted
    }
}
