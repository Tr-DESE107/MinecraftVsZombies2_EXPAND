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
            LoadUserData(userDataList.CurrentUserIndex);
        }
        public void LoadUserData(int index)
        {
            modSaveDatas.Clear();
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                LoadModData(index, mod.Namespace);
            }
            EvaluateUnlocks();
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
            EvaluateUnlocks();
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

        #region 私有方法
        private void EvaluateUnlocks()
        {
            EvaluateUnlockedEntities();
            EvaluateUnlockedArtifacts();
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
        #endregion

        #region 属性字段
        public MainManager Main => MainManager.Instance;
        private List<ModSaveData> modSaveDatas = new List<ModSaveData>();
        private List<NamespaceID> unlockedContraptionsCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedEnemiesCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedArtifactsCache = new List<NamespaceID>();
        #endregion
    }
}
