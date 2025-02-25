using System.Collections.Generic;
using System.IO;
using System.Linq;
using MVZ2.Sprites;
using UnityEditor;
using UnityEditor.U2D.Sprites;
using UnityEngine;

namespace MVZ2.Editor
{
    public class SpriteMenu : MonoBehaviour
    {
        public static Sprite[] GetOrderedSpriteSheet(string path, SpriteDataProviderFactories factories)
        {
            var textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
            var dataProvider = factories.GetSpriteEditorDataProviderFromObject(textureImporter);

            dataProvider.InitSpriteEditorDataProvider();
            var spriteRects = dataProvider.GetSpriteRects();//if you don't have the initial sprite rects, just create them manually, this example just changes the pivot points
            var spriteOrders = spriteRects.Select((rect, index) => (rect.name, index)).ToDictionary(p => p.name, p => p.index);
            return AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>().OrderBy(s => spriteOrders[s.name]).ToArray();
        }
        public static void UpdateSpriteManifest(string manifestPath)
        {
            var manifestAssetPath = AssetsMenu.FileToAssetPath(manifestPath);
            var spriteManifest = AssetDatabase.LoadAssetAtPath<SpriteManifest>(manifestAssetPath);
            if (!spriteManifest)
            {
                Debug.LogError("Sprite manifest of script AddressablesMenu is not set.");
                return;
            }
            Debug.Log("Updating Sprite Manifest...");
            var directory = GetSpriteAssetsDirectory();
            //var directory = Path.Combine(Application.dataPath, "GameContent", "Sprites");
            var filePaths = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
            spriteManifest.spriteEntries.Clear();
            spriteManifest.spritesheetEntries.Clear();

            var factories = new SpriteDataProviderFactories();
            factories.Init();

            foreach (string filePath in filePaths)
            {
                var relativePath = Path.GetRelativePath(directory, filePath);
                var splitPath = relativePath.Split('/', '\\');
                bool isSheet = splitPath[0] == "spritesheets";
                var resourceDirectory = string.Join('/', splitPath.Skip(1).SkipLast(1));
                var resourceName = Path.Combine(resourceDirectory, Path.GetFileNameWithoutExtension(filePath)).Replace("\\", "/");
                var assetPath = AssetsMenu.FileToAssetPath(filePath);
                var sprites = GetOrderedSpriteSheet(assetPath, factories);
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
            }
            AssetDatabase.SaveAssetIfDirty(spriteManifest);
            Debug.Log($"Update sprite manifest completed.");

            EditorUtility.ClearProgressBar();
        }
        [MenuItem("Custom/Assets/Sprites/Update Sprite At File")]
        public static void UpdateSpriteManifestAtFile()
        {
            var manifestPath = EditorUtility.OpenFilePanelWithFilters("Open sprite manifest", "Assets/GameContent/Assets", new string[] { "Sprite Manifest", "asset" });
            UpdateSpriteManifest(manifestPath);
        }
        [MenuItem("Custom/Assets/Sprites/Update Sprite Manifest")]
        public static void UpdateSpriteManifestAtGameContent()
        {
            var manifestPath = Path.Combine(Application.dataPath, "GameContent", "Assets", "mvz2", "spritemanifests", "manifest.asset");
            UpdateSpriteManifest(manifestPath);
        }
        [MenuItem("Custom/Assets/Sprites/Rename Sprites")]
        public static void RenameSprites()
        {
            Debug.Log("Renaming Sprites...");
            var directory = GetSpriteAssetsDirectory();
            var filePaths = Directory.GetFiles(directory, "*.png", SearchOption.AllDirectories);
            var renamedDict = new Dictionary<string, string>();
            var rootAssetPath = AssetsMenu.FileToAssetPath(directory);

            var factory = new SpriteDataProviderFactories();
            factory.Init();

            List<string> dirtySprites = new List<string>();

            for (int i = 0; i < filePaths.Length; i++)
            {
                var filePath = filePaths[i];
                var relativePath = Path.GetRelativePath(directory, filePath);
                var splitPath = relativePath.Split('/', '\\');

                var assetPath = AssetsMenu.FileToAssetPath(filePath);
                var texture2D = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath);

                var textureImporter = AssetImporter.GetAtPath(assetPath) as TextureImporter;
                var dataProvider = factory.GetSpriteEditorDataProviderFromObject(textureImporter);

                dataProvider.InitSpriteEditorDataProvider();
                var spriteRects = dataProvider.GetSpriteRects();//if you don't have the initial sprite rects, just create them manually, this example just changes the pivot points

                bool dirty = false;
                bool isSheet = spriteRects.Length > 1;
                for (int r = 0; r < spriteRects.Length; r++)
                {
                    var format = isSheet ? "{0}_{1}" : "{0}";
                    var oldName = spriteRects[r].name;
                    var newName = string.Format(format, texture2D.name, r);
                    if (newName == oldName)
                    {
                        continue;
                    }
                    spriteRects[r].name = newName;
                    dirty = true;
                    renamedDict.Add(oldName, newName);
                }
                if (!dirty)
                    continue;
                dataProvider.SetSpriteRects(spriteRects);
                dataProvider.Apply();
                dirtySprites.Add(filePath);

                EditorUtility.SetDirty(textureImporter);
                textureImporter.SaveAndReimport();
                EditorUtility.DisplayProgressBar("Renaming sprites...", filePath, (float)i / filePaths.Length);
            }
            foreach (var filePath in dirtySprites)
            {
                AssetDatabase.ImportAsset(filePath, ImportAssetOptions.ForceUpdate);
            }
            Debug.Log($"Rename sprites completed, expected {filePaths.Length}, updated {dirtySprites.Count}.");
            Debug.Log($"Renamed sprites:\n{string.Join("\n", renamedDict.Select(p => $"{p.Key} => {p.Value}"))}");

            EditorUtility.ClearProgressBar();
        }
        public static string GetSpriteAssetsDirectory()
        {
            return Path.Combine(Application.dataPath, "GameContent", "Assets", "mvz2");
        }
    }
}
