using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Managers;
using MVZ2.Serialization;
using PVZEngine;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2.Level
{
    public class LevelManager : MonoBehaviour, ILevelManager
    {
        public LevelController GetLevel()
        {
            return controller;
        }
        public void InitLevel(NamespaceID areaID, NamespaceID stageID, float beginningDelay = 0)
        {
            if (!controller)
                return;
            Main.SaveManager.SaveModDatas();
            controller.StartStageID = stageID;
            controller.StartAreaID = areaID;
            if (HasLevelState(stageID))
            {
                LoadLevel(areaID, stageID);
            }
            else
            {
                controller.InitLevel(Main.Game, areaID, stageID);
                controller.StartLevelIntro(beginningDelay);
            }
        }
        #region 关卡存读
        public void SaveLevel()
        {
            if (!controller)
                return;
            var path = GetLevelStatePath(controller.GetStartStageID());
            FileHelper.ValidateDirectory(path);
            var seri = controller.SaveGame();
            var json = seri.ToBson();
            Main.FileManager.WriteJsonFile(path, json);
        }
        public void LoadLevel(NamespaceID areaID, NamespaceID stageID)
        {
            if (!controller)
                return;
            if (!HasLevelState(stageID))
                return;
            try
            {
                var path = GetLevelStatePath(stageID);
                var json = Main.FileManager.ReadJsonFile(path);
                var seri = SerializeHelper.FromBson<SerializableLevelController>(json);
                controller.LoadGame(seri, Main.Game, areaID, stageID);
            }
            catch (Exception e)
            {
                controller.ShowLevelErrorLoadingDialog(e);
            }
        }
        public bool HasLevelState(NamespaceID stageID)
        {
            var path = GetLevelStatePath(stageID);
            return File.Exists(path);
        }
        public void RemoveLevelState(NamespaceID stageID)
        {
            var path = GetLevelStatePath(stageID);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        public string GetLevelStatePath(NamespaceID stageID)
        {
            var userIndex = Main.SaveManager.GetCurrentUserIndex();
            var dir = Main.SaveManager.GetUserModSaveDataDirectory(userIndex, stageID.spacename);
            return Path.Combine(dir, "level", $"{stageID.path}.lvl");
        }
        public LevelDataIdentifierList GetLevelStateIdentifierList()
        {
            var mods = Main.ModManager.GetAllModInfos();
            return new LevelDataIdentifierList(mods.Select(m => new LevelDataIdentifier(m.Namespace, m.LevelDataVersion)));
        }
        #endregion
        public async Task GotoLevelSceneAsync()
        {
            var sceneName = "Level";
            var oldScene = Scene.GetSceneInstance(sceneName);
            await Scene.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (oldScene.Scene.IsValid())
            {
                await Scene.UnloadSceneAsync(oldScene);
            }

            var newScene = Scene.GetSceneInstance(sceneName);
            foreach (var go in newScene.Scene.GetRootGameObjects())
            {
                var ctrl = go.GetComponent<LevelController>();
                if (ctrl)
                {
                    controller = ctrl;
                    break;
                }
            }
        }
        public async Task ExitLevelSceneAsync()
        {
            var sceneName = "Level";
            if (!Scene.IsSceneLoaded(sceneName))
                return;
            await Scene.UnloadSceneAsync(sceneName);
            controller = null;
        }
        public const int CURRENT_DATA_VERSION = 0;
        public float LawnToTransScale => 1 / transToLawnScale;
        public float TransToLawnScale => transToLawnScale;
        public MainManager Main => main;
        public SceneLoadingManager Scene => main.SceneManager;
        [SerializeField]
        private MainManager main;
        [SerializeField]
        private float transToLawnScale = 100;
        private LevelController controller;
    }
    [Serializable]
    public class LevelDataIdentifierList
    {
        public LevelDataIdentifierList(IEnumerable<LevelDataIdentifier> identifiers)
        {
            this.identifiers.AddRange(identifiers);
        }
        public bool Compare(LevelDataIdentifierList list)
        {
            if (list == null)
                return false;
            if (list.identifiers.Count != identifiers.Count)
                return false;
            return list.identifiers.Any(i => identifiers.Contains(i));
        }
        public override int GetHashCode()
        {
            var hash = 0;
            foreach (var id in identifiers)
            {
                hash = hash * 31 + id.GetHashCode();
            }
            return hash;
        }
        public List<LevelDataIdentifier> identifiers = new List<LevelDataIdentifier>();
    }
    [Serializable]
    public class LevelDataIdentifier
    {
        public LevelDataIdentifier(string name, int version)
        {
            spaceName = name;
            dataVersion = version;
        }
        public string spaceName;
        public int dataVersion;

        public override bool Equals(object obj)
        {
            return obj is LevelDataIdentifier identifier &&
                   spaceName == identifier.spaceName &&
                   dataVersion == identifier.dataVersion;
        }

        public override int GetHashCode()
        {
            var hash = spaceName.GetHashCode();
            hash = hash * 31 + dataVersion.GetHashCode();
            return hash;
        }

        public static bool operator ==(LevelDataIdentifier left, LevelDataIdentifier right)
        {
            return EqualityComparer<LevelDataIdentifier>.Default.Equals(left, right);
        }

        public static bool operator !=(LevelDataIdentifier left, LevelDataIdentifier right)
        {
            return !(left == right);
        }
    }
}
