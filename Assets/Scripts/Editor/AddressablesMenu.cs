using System.Collections.Generic;
using System.IO;
using System.Linq;
using PVZEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MVZ2.Editor
{
    public class AddressablesMenu : MonoBehaviour
    {
        [MenuItem("Custom/Addressables/Update Addressables")]
        public static void UpdateAddressables()
        {
            Debug.Log("Updating Addressables...");
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var settingLabels = settings.GetLabels();

            ClearAddressables();

            var directory = Path.Combine(Application.dataPath, "GameContent");
            var filePaths = new List<string>();
            SearchFilesForUpdate(directory, filePaths);

            List<string> markedList = new List<string>();
            foreach (string path in filePaths)
            {
                if (!MarkResource(directory, path))
                    continue;
                markedList.Add(path);
                EditorUtility.DisplayProgressBar("Marking resources as addressables...", path, (float)markedList.Count / filePaths.Count);
            }

            Debug.Log($"Update Addressables completed, expected {filePaths.Count}, updated {markedList.Count}." +
                $"{(markedList.Count < filePaths.Count ? $"\nNot updated: \n{string.Join("\n", filePaths.Except(markedList))}" : string.Empty)}");

            EditorUtility.ClearProgressBar();
            EditorUtility.RequestScriptReload();
        }
        private static void SearchFilesForUpdate(string folder, List<string> pathList)
        {
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);

            int progress = 0;
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file);
                if (extension == ".meta")
                    continue;

                if (extension == ".unity")
                {
                    if (EditorBuildSettings.scenes.Any(s => s.path == file.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)))
                        continue;
                }
                pathList.Add(file);
                EditorUtility.DisplayProgressBar("Checking markable assets...", folder, progress / (float)files.Length);
            }
        }

        private static void ClearAddressables()
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.DefaultGroup;
            var entries = group.entries.ToArray();
            foreach (var asset in entries)
            {
                group.RemoveAssetEntry(asset, postEvent: false);
            }
        }
        private static bool MarkResource(string rootDirectory, string filePath)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var assetPath = Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, filePath));
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                AddressableAssetGroup group = settings.DefaultGroup;
                entry = settings.CreateOrMoveEntry(guid, group, postEvent: false);
            }
            var relativePath = Path.GetRelativePath(rootDirectory, filePath);
            SetEntryAddressAndLabels(entry, relativePath);
            return true;
        }
        private static void SetEntryAddressAndLabels(AddressableAssetEntry entry, string relativePath)
        {
            var splitedPaths = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var labels = new List<string>();
            if (splitedPaths.Length >= 2)
            {
                string keyRoot = splitedPaths[0];
                string nsp = null;
                if (splitedPaths.Length >= 4 && splitedPaths[0].ToLower() == "assets")
                {
                    nsp = splitedPaths[1];
                    var type = splitedPaths[2];
                    switch (type.ToLower())
                    {
                        case "metas":
                            labels.Add("Meta");
                            break;
                        case "lang":
                            labels.Add("Language");
                            break;
                        case "music":
                            labels.Add("Music");
                            break;
                        case "sounds":
                            labels.Add("Sound");
                            break;
                        case "models":
                            labels.Add("Model");
                            break;
                        case "sprites":
                            labels.Add("Sprite");
                            break;
                        case "spritesheets":
                            labels.Add("Spritesheet");
                            break;
                    }
                    keyRoot = Path.Combine(splitedPaths[0], nsp, type);
                }
                var keyPath = Path.GetRelativePath(keyRoot, relativePath).Replace("\\", "/");
                var idPath = Path.ChangeExtension(keyPath, "").TrimEnd('.');
                entry.SetAddress(new NamespaceID(nsp, idPath).ToString(), postEvent: false);
            }

            foreach (var label in labels)
            {
                entry.SetLabel(label, true, postEvent: false);
            }

        }
    }
}
