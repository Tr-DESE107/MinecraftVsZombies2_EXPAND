using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MVZ2.Editor
{
    public class AddressablesMenu
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
            foreach (var asset in group.entries.ToArray())
            {
                group.RemoveAssetEntry(asset);
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
                entry = settings.CreateOrMoveEntry(guid, group);
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
                if (splitedPaths.Length >= 3 && splitedPaths[0] == "Assets")
                {
                    switch (splitedPaths[1])
                    {
                        case "Metas":
                            labels.Add("Meta");
                            break;
                        case "Sounds":
                            labels.Add("Sound");
                            break;
                        case "Models":
                            labels.Add("Model");
                            break;
                        case "Textures":
                            if (entry.SubAssets != null && entry.SubAssets.Count > 0)
                                labels.Add("Spritesheet");
                            else
                                labels.Add("Sprite");
                            break;
                    }
                    keyRoot = Path.Combine(splitedPaths[0], splitedPaths[1]);
                }
                var keyPath = Path.GetRelativePath(keyRoot, relativePath).Replace("\\", "/");
                var key = Path.ChangeExtension(keyPath, "").TrimEnd('.');
                entry.address = key;
            }

            foreach (var label in labels)
            {
                entry.SetLabel(label, true);
            }

        }
    }
}
