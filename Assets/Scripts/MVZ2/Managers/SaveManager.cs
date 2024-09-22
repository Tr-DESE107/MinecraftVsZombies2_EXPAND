using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVZ2.Save;
using PVZEngine;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2
{
    public partial class SaveManager : MonoBehaviour
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
            var serializable = modSaveData.ToSerializable();
            var metaJson = serializable.ToBson();
            SerializeHelper.WriteCompressedJson(path, metaJson);
        }
        #endregion

        #region 加载
        public void Load()
        {
            LoadUserList();
            ReloadCurrentUserData();
        }
        public void ReloadCurrentUserData()
        {
            foreach (var mod in Main.ModManager.GetAllModInfos())
            {
                LoadModData(userDataList.CurrentUserIndex, mod.Namespace);
            }
            EvaluateUnlockedEntities();
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
                var saveDataJson = SerializeHelper.ReadCompressedJson(path);
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
        public bool IsUnlocked(NamespaceID stageID)
        {
            if (stageID == null)
                return false;
            var modSaveData = GetModSaveData(stageID.spacename);
            if (modSaveData == null)
                return false;
            return modSaveData.IsUnlocked(stageID.path);
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
        public NamespaceID[] GetUnlockedContraptions()
        {
            return unlockedContraptionsCache.ToArray();
        }
        public NamespaceID[] GetUnlockedEnemies()
        {
            return unlockedEnemiesCache.ToArray();
        }
        #endregion

        #region 修改
        public bool DeleteUserSaveData(int index)
        {
            var path = GetUserSaveDataDirectory(index);
            if (!Directory.Exists(path))
                return false;
            Directory.Delete(path, true);
            return true;
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
        public MainManager Main => MainManager.Instance;
        private List<ModSaveData> modSaveDatas = new List<ModSaveData>();
        private List<NamespaceID> unlockedContraptionsCache = new List<NamespaceID>();
        private List<NamespaceID> unlockedEnemiesCache = new List<NamespaceID>();
        #endregion
    }
}
