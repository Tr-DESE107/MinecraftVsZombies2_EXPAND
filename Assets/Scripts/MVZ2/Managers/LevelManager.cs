using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MVZ2.Level;
using PVZEngine;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MVZ2
{
    public class LevelManager : MonoBehaviour
    {
        public LevelController GetLevel()
        {
            return controller;
        }
        public void StartLevel(NamespaceID areaID, NamespaceID stageID)
        {
            if (!controller)
                return;
            controller.StartStageID = stageID;
            controller.StartAreaID = areaID;
            if (HasLevelState(stageID))
            {
                LoadLevel(stageID);
            }
            else
            {
                controller.InitGame(Main.Game, areaID, stageID);
            }
        }
        #region 关卡存读
        public void SaveLevel()
        {
            if (!controller)
                return;
            var path = GetLevelStatePath(controller.StartStageID);
            FileHelper.ValidateDirectory(path);
            var seri = controller.SaveGame();
            var json = seri.ToBson(true);
            SerializeHelper.WriteJson(path, json);
        }
        public void LoadLevel(NamespaceID stageID)
        {
            if (!controller)
                return;
            if (!HasLevelState(stageID))
                return;
            try
            {
                var path = GetLevelStatePath(stageID);
                var json = SerializeHelper.ReadJson(path);
                var seri = SerializeHelper.FromBson<SerializableLevelController>(json);
                controller.LoadGame(seri, Main.Game);
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
        public NamespaceID[] GetSeedPacksID()
        {
            var packs = new NamespaceID[6];
            var unlocked = Main.SaveManager.GetUnlockedContraptions();
            for (int i = 0; i < packs.Length; i++)
            {
                if (i < unlocked.Length)
                {
                    packs[i] = unlocked[i];
                }
            }
            return packs;
        }
        public async Task GotoLevelSceneAsync()
        {
            var sceneName = "Level";
            var oldScene = GetScene(sceneName);
            await LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            if (oldScene.IsValid())
            {
                await UnloadSceneAsync(oldScene);
            }

            var newScene = GetScene(sceneName);
            foreach (var go in newScene.GetRootGameObjects())
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
            if (!IsSceneLoaded(sceneName))
                return;
            await UnloadSceneAsync(sceneName);
            controller = null;
        }
        private bool IsSceneLoaded(string name)
        {
            var scene = SceneManager.GetSceneByName(name);
            return scene.IsValid();
        }
        private Scene GetScene(string name)
        {
            return SceneManager.GetSceneByName(name);
        }
        private Task LoadSceneAsync(string name, LoadSceneMode mode)
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.LoadSceneAsync(name, mode);
            op.completed += (op) => tcs.SetResult(op);

            return tcs.Task;
        }
        private Task UnloadSceneAsync(string name)
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.UnloadSceneAsync(name);
            op.completed += (op) => tcs.SetResult(op);

            return tcs.Task;
        }
        private Task UnloadSceneAsync(Scene scene)
        {
            TaskCompletionSource<AsyncOperation> tcs = new TaskCompletionSource<AsyncOperation>();
            var op = SceneManager.UnloadSceneAsync(scene);
            op.completed += (op) => tcs.SetResult(op);

            return tcs.Task;
        }
        public const int CURRENT_DATA_VERSION = 0;
        public MainManager Main => main;
        [SerializeField]
        private MainManager main;
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
