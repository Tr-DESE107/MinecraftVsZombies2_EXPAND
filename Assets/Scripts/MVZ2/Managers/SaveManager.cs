using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVZ2.GameContent;
using MVZ2.Save;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2
{
    public class SaveManager : MonoBehaviour
    {
        public void SaveMetaList()
        {
            var saveDataMetaPath = GetSaveDataMetaPath();
            FileHelper.ValidateDirectory(saveDataMetaPath);
            var serializable = saveDataMetaList.ToSerializable();
            var metaJson = serializable.ToBson();
            SerializeHelper.WriteCompressedJson(saveDataMetaPath, metaJson);
        }
        public void SaveModDatas()
        {
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                SaveModData(saveDataMetaList.CurrentUserIndex, mod.Namespace);
            }
        }
        public void SaveModData(int userIndex, string spaceName)
        {
            var path = GetSaveDataPath(userIndex, spaceName);
            FileHelper.ValidateDirectory(path);
            var modSaveData = GetModSaveData(spaceName);
            var serializable = modSaveData.ToSerializable();
            var metaJson = serializable.ToBson();
            SerializeHelper.WriteCompressedJson(path, metaJson);
        }
        public void Load()
        {
            LoadMetaList();
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                LoadModData(saveDataMetaList.CurrentUserIndex, mod.Namespace);
            }
            EvaluateUnlockedEntities();
        }
        public bool IsPrologueCleared()
        {
            return GetLevelDifficultyRecords(BuiltinStageID.prologue).Length > 0;
        }
        public ModSaveData GetModSaveData(string spaceName)
        {
            return modSaveDatas.FirstOrDefault(s => s.Namespace == spaceName);
        }
        public T GetModSaveData<T>(string spaceName) where T : ModSaveData
        {
            foreach (var saveData in modSaveDatas)
            {
                if (saveData.Namespace == spaceName && saveData is T tData)
                {
                    return tData;
                }
            }
            return null;
        }
        public void SetCurrentUserName(string name)
        {
            var index = saveDataMetaList.CurrentUserIndex;
            var meta = saveDataMetaList.GetUserMeta(index);
            if (meta == null)
            {
                meta = saveDataMetaList.CreateUserMeta(index);
            }
            meta.Username = name;
        }
        public string GetCurrentUserName()
        {
            var index = saveDataMetaList.CurrentUserIndex;
            var meta = saveDataMetaList.GetUserMeta(index);
            if (meta == null)
                return null;
            return meta.Username;
        }
        public int GetCurrentUserIndex()
        {
            return saveDataMetaList.CurrentUserIndex;
        }
        public bool HasDuplicateUserName(string name, int indexToExcept)
        {
            for (int i = 0; i < saveDataMetaList.GetMaxUserCount(); i++)
            {
                if (i == indexToExcept)
                    continue;
                var meta = saveDataMetaList.GetUserMeta(i);
                if (meta == null)
                    continue;
                if (meta.Username == name)
                    return true;
            }
            return false;
        }


        #region 路径
        public string GetSaveDataRoot()
        {
            return Path.Combine(Application.persistentDataPath, "userdata");
        }
        public string GetSaveDataMetaPath()
        {
            return Path.Combine(GetSaveDataRoot(), "users.dat");
        }
        public string GetSaveDataDirectory(int userIndex)
        {
            return Path.Combine(GetSaveDataRoot(), $"user{userIndex}");
        }
        public string GetSaveDataPath(int userIndex, string spaceName)
        {
            return Path.Combine(GetSaveDataDirectory(userIndex), spaceName);
        }
        #endregion

        #region 解锁状态
        public bool IsUnlocked(NamespaceID stageID)
        {
            if (stageID == null)
                return false;
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return false;
            return modSaveData.IsUnlocked(stageID.path);
        }
        #endregion

        #region 关卡难度记录
        public NamespaceID[] GetLevelDifficultyRecords(NamespaceID stageID)
        {
            if (stageID == null)
                return Array.Empty<NamespaceID>();
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return Array.Empty<NamespaceID>();
            return modSaveData.GetLevelDifficultyRecords(stageID.path);
        }
        #endregion

        #region 解锁实体
        public NamespaceID[] GetUnlockedContraptions()
        {
            return unlockedContraptionsCache.ToArray();
        }
        public NamespaceID[] GetUnlockedEnemies()
        {
            return unlockedEnemiesCache.ToArray();
        }
        #endregion

        #region 私有方法
        private void LoadMetaList()
        {
            var saveDataMetaPath = GetSaveDataMetaPath();
            if (!File.Exists(saveDataMetaPath))
            {
                saveDataMetaList = new SaveDataMetaList(MAX_USER_COUNT);
            }
            else
            {
                var metaJson = SerializeHelper.ReadCompressedJson(saveDataMetaPath);
                var serializable = SerializeHelper.FromBson<SerializableSaveDataMetaList>(metaJson);
                saveDataMetaList = SaveDataMetaList.FromSerializable(serializable);
            }
        }
        private void LoadModData(int userIndex, string spaceName)
        {
            var path = GetSaveDataPath(userIndex, spaceName);
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
                var saveDataJson = SerializeHelper.ReadCompressedJson(path);
                saveData = modInfo.Logic.LoadSaveData(saveDataJson);
            }
            modSaveDatas.Add(saveData);
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
                if (NamespaceID.IsValid(meta.unlock) && !IsUnlocked(meta.unlock))
                    continue;
                if (meta.type == EntityTypes.PLANT)
                {
                    unlockedContraptionsCache.Add(id);
                }
                else if (meta.type == EntityTypes.ENEMY)
                {
                    unlockedEnemiesCache.Add(id);
                }
            }
        }
        #endregion

        #region 属性字段
        public const int MAX_USER_COUNT = 8;
        public MainManager Main => MainManager.Instance;
        private SaveDataMetaList saveDataMetaList;
        private List<ModSaveData> modSaveDatas = new List<ModSaveData>();
        private List<NamespaceID> unlockedContraptionsCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedEnemiesCache = new List<NamespaceID>();
        #endregion
    }
}
