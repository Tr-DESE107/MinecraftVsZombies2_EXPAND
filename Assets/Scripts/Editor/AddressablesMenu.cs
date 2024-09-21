using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using PVZEngine;
using UnityEditor.U2D.Sprites;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MVZ2.Editor
{
    public class AddressablesMenu : MonoBehaviour
    {
        public static void UpdateSpriteManifest(string manifestPath)
        {
            var manifestAssetPath = FileToAssetPath(manifestPath);
            var spriteManifest = AssetDatabase.LoadAssetAtPath<SpriteManifest>(manifestAssetPath);
            if (!spriteManifest)
            {
                Debug.LogError("Sprite manifest of script AddressablesMenu is not set.");
                return;
            }
            Debug.Log("Updating Sprite Manifest...");
            var directory = Path.Combine(Application.dataPath, "GameContent", "Assets", "mvz2");
            //var directory = Path.Combine(Application.dataPath, "GameContent", "Sprites");
            var filePaths = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
            spriteManifest.spriteEntries.Clear();
            spriteManifest.spritesheetEntries.Clear();

            foreach (string filePath in filePaths)
            {
                var relativePath = Path.GetRelativePath(directory, filePath);
                var splitPath = relativePath.Split('/', '\\');
                bool isSheet = splitPath[0] == "spritesheets";
                var resourceDirectory = string.Join('/', splitPath.Skip(1).SkipLast(1));
                var resourceName = Path.Combine(resourceDirectory, Path.GetFileNameWithoutExtension(filePath)).Replace("\\", "/");
                var assetPath = FileToAssetPath(filePath);
                var sprites = AssetDatabase.LoadAllAssetsAtPath(assetPath).OfType<Sprite>().ToArray();
                if (isSheet)
                {
                    spriteManifest.spritesheetEntries.Add(new SpriteSheetEntry()
                    {
                        name = resourceName,
                        spritesheet = sprites.ToArray()
                    });
                }
                else if (sprites.Length == 1)
                {
                    var sprite = sprites[0];
                    spriteManifest.spriteEntries.Add(new SpriteEntry()
                    {
                        name = resourceName,
                        sprite = sprite
                    });
                }
                EditorUtility.SetDirty(spriteManifest);
                AssetDatabase.SaveAssetIfDirty(spriteManifest);
            }
            Debug.Log($"Update sprite manifest completed.");

            EditorUtility.ClearProgressBar();
        }
        [MenuItem("Custom/Addressables/Update Sprite At File")]
        public static void UpdateSpriteManifestAtFile()
        {
            var manifestPath = EditorUtility.OpenFilePanelWithFilters("Open sprite manifest", "Assets/GameContent/Assets", new string[] { "Sprite Manifest", "asset" });
            UpdateSpriteManifest(manifestPath);
        }
        [MenuItem("Custom/Addressables/Update Sprite Manifest")]
        public static void UpdateSpriteManifestAtGameContent()
        {
            var manifestPath = Path.Combine(Application.dataPath, "GameContent", "Assets", "mvz2", "spritemanifests", "manifest.asset");
            UpdateSpriteManifest(manifestPath);
        }
        [MenuItem("Custom/Addressables/Rename Sprites")]
        public static void RenameSprites()
        {
            Debug.Log("Renaming Sprites...");
            var directory = Path.Combine(Application.dataPath, "GameContent", "Sprites");
            var filePaths = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
            var renamedDict = new Dictionary<string, string>();
            var rootAssetPath = FileToAssetPath(directory);

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            foreach (string filePath in filePaths)
            {
                var relativePath = Path.GetRelativePath(directory, filePath);
                var splitPath = relativePath.Split('/', '\\');

                var assetPath = FileToAssetPath(filePath);
                var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);
                var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);

                dataProvider.InitSpriteEditorDataProvider();
                var spriteRects = dataProvider.GetSpriteRects();//if you don't have the initial sprite rects, just create them manually, this example just changes the pivot points
                bool isSheet = spriteRects.Length > 1;
                for (int i = 0; i < spriteRects.Length; i++)
                {
                    var suffix = isSheet ? $"[{i}]" : string.Empty;
                    spriteRects[i].name = $"{texture2D.name}{suffix}";
                }
                dataProvider.SetSpriteRects(spriteRects);
                dataProvider.Apply();

                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
                AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
                EditorUtility.DisplayProgressBar("Renaming sprites...", filePath, (float)renamedDict.Count / filePaths.Length);
            }
            Debug.Log($"Rename sprites completed, expected {filePaths.Length}, updated {renamedDict.Count}.");
            Debug.Log($"Renamed sprites:\n{string.Join("\n", renamedDict.Select(p => $"{p.Key} => {p.Value}"))}");

            EditorUtility.ClearProgressBar();
        }
        [MenuItem("Custom/Addressables/Update Addressables")]
        public static void UpdateAddressables()
        {
            Debug.Log("Updating Addressables...");

            var settings = AddressableAssetSettingsDefaultObject.Settings;

            HashSet<string> dependencies = new HashSet<string>();

            var mainGroup = settings.DefaultGroup;
            var languagePackGroup = settings.groups.FirstOrDefault(g => g.name == "language_packs");
            var dependenciesGroup = settings.groups.FirstOrDefault(g => g.name == "dependencies");
            ClearAddressables(mainGroup);
            ClearAddressables(languagePackGroup);
            ClearAddressables(dependenciesGroup);

            var mainDirectory = Path.Combine(Application.dataPath, "GameContent", "Assets");
            var assetMarker = new MainAssetMarker();
            UpdateAddressableGroup(mainGroup, mainDirectory, assetMarker, dependencies);

            var sceneDirectory = Path.Combine(Application.dataPath, "GameContent", "Scenes");
            var sceneMarker = new SceneMarker();
            UpdateAddressableGroup(mainGroup, sceneDirectory, sceneMarker, dependencies);

            var languagePackDirectory = LocalizationMenu.GetLanguagePackDirectory();
            var languagePackMarker = new LanguagePackMarker();
            UpdateAddressableGroup(languagePackGroup, languagePackDirectory, languagePackMarker, dependencies);

            UpdateDependenciesGroup(dependenciesGroup, dependencies);

            Debug.Log($"Update Addressables completed.");

            EditorUtility.ClearProgressBar();
            EditorUtility.RequestScriptReload();
        }
        [MenuItem("Custom/Update Resources")]
        public static void UpdateResources()
        {
            LocalizationMenu.CompressLanguagePack();
            UpdateSpriteManifestAtGameContent();
            UpdateAddressables();
        }
        private static void UpdateAddressableGroup(AddressableAssetGroup group, string directory, AssetMarker marker, HashSet<string> dependencies)
        {
            var filePaths = new List<string>();
            SearchFilesForUpdate(directory, filePaths);
            List<string> markedList = new List<string>();
            foreach (string path in filePaths)
            {
                if (!MarkResource(group, directory, path, marker))
                    continue;
                var assetPath = FileToAssetPath(path);
                foreach (var dep in AssetDatabase.GetDependencies(assetPath))
                {
                    dependencies.Add(dep);
                }
                markedList.Add(path);
                EditorUtility.DisplayProgressBar("Marking resources as addressables...", path, (float)markedList.Count / filePaths.Count);
            }
        }
        private static void UpdateDependenciesGroup(AddressableAssetGroup group, IEnumerable<string> assetPaths)
        {
            var totalCount = assetPaths.Count();
            List<string> markedList = new List<string>();
            foreach (string assetPath in assetPaths)
            {
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                string guid = AssetDatabase.AssetPathToGUID(assetPath);
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                if (!asset || asset.hideFlags.HasFlag(HideFlags.DontSave))
                    continue;
                AddressableAssetEntry entry = settings.FindAssetEntry(guid);
                if (entry == null)
                {
                    entry = settings.CreateOrMoveEntry(guid, group, postEvent: false);
                }
                markedList.Add(assetPath);
                EditorUtility.DisplayProgressBar("Marking resources as addressables...", assetPath, (float)markedList.Count / totalCount);
            }
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

        private static void ClearAddressables(AddressableAssetGroup group)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var entries = group.entries.ToArray();
            foreach (var asset in entries)
            {
                group.RemoveAssetEntry(asset, postEvent: false);
            }
        }
        private static bool MarkResource(AddressableAssetGroup group, string rootDirectory, string filePath, AssetMarker marker)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            var assetPath = FileToAssetPath(filePath);
            string guid = AssetDatabase.AssetPathToGUID(assetPath);
            AddressableAssetEntry entry = settings.FindAssetEntry(guid);
            if (entry == null)
            {
                entry = settings.CreateOrMoveEntry(guid, group, postEvent: false);
            }
            var relativePath = Path.GetRelativePath(rootDirectory, filePath);
            marker.MarkAsset(entry, relativePath);
            return true;
        }
        private static string FileToAssetPath(string filePath)
        {
            return Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, filePath));
        }
    }
    public abstract class AssetMarker
    {
        public abstract void MarkAsset(AddressableAssetEntry entry, string relativePath);
    }
    public class MainAssetMarker : AssetMarker
    {
        private string[] GetEntryLabels(string type)
        {
            var labels = new List<string>();
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
                case "spritemanifests":
                    labels.Add("SpriteManifest");
                    break;
            }
            return labels.ToArray();
        }
        public override void MarkAsset(AddressableAssetEntry entry, string relativePath)
        {
            var splitedPaths = relativePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            if (splitedPaths.Length >= 2)
            {
                string keyRoot = splitedPaths[0];
                string nsp = null;
                if (splitedPaths.Length >= 3)
                {
                    nsp = splitedPaths[0];
                    var type = splitedPaths[1];
                    keyRoot = Path.Combine(nsp, type);

                    var labels = GetEntryLabels(type);
                    foreach (var label in labels)
                    {
                        entry.SetLabel(label, true, postEvent: false);
                    }
                }
                var keyPath = Path.GetRelativePath(keyRoot, relativePath).Replace("\\", "/");
                var idPath = Path.ChangeExtension(keyPath, "").TrimEnd('.');
                entry.SetAddress(new NamespaceID(nsp, idPath).ToString(), postEvent: false);
            }
        }
    }
    public class LanguagePackMarker : AssetMarker
    {
        public override void MarkAsset(AddressableAssetEntry entry, string relativePath)
        {
            var idPath = Path.GetFileNameWithoutExtension(relativePath);
            entry.SetAddress(idPath, postEvent: false);
            entry.SetLabel("LanguagePack", true, postEvent: false);
        }
    }
    public class SceneMarker : AssetMarker
    {
        public override void MarkAsset(AddressableAssetEntry entry, string relativePath)
        {
            var idPath = Path.GetFileNameWithoutExtension(relativePath);
            entry.SetAddress(idPath, postEvent: false);
            entry.SetLabel("Scene", true, postEvent: false);
        }
    }
    public class DummyMarker : AssetMarker
    {
        public override void MarkAsset(AddressableAssetEntry entry, string relativePath)
        {
        }
    }
}
