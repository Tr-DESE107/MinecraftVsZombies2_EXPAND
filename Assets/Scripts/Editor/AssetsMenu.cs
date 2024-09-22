using System.IO;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    public static class AssetsMenu
    {
        public static void UpdateAllAssets()
        {
            LocalizationMenu.CompressLanguagePack();
            SpriteMenu.RenameSprites();
            SpriteMenu.UpdateSpriteManifestAtGameContent();
            AddressablesMenu.UpdateAddressables();
        }
        [MenuItem("Custom/Assets/Update All Assets")]
        public static void UpdateAllAssetsMenu()
        {
            UpdateAllAssets();
            EditorUtility.RequestScriptReload();
        }
        public static string FileToAssetPath(string filePath)
        {
            return Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, filePath));
        }
    }
}
