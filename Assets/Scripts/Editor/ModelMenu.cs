using System.Collections.Generic;
using System.IO;
using MVZ2.Rendering;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    public class ModelMenu
    {
        [MenuItem("Custom/Assets/Models/Update Model Elements")]
        public static void UpdateModelElements()
        {
            Debug.Log("Updating model elements...");
            var directory = Path.Combine(Application.dataPath, "GameContent", "Assets", "mvz2", "models");
            var filePaths = new List<string>();
            SearchModelPaths(directory, filePaths);

            foreach (string path in filePaths)
            {
                UpdateModelElement(Path.Combine("Assets", Path.GetRelativePath(Application.dataPath, path)));
            }

            Debug.Log($"Update model elements completed.");
        }
        private static void SearchModelPaths(string folder, List<string> pathList)
        {
            string[] files = Directory.GetFiles(folder, "*", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                string extension = Path.GetExtension(file);
                if (extension != ".prefab")
                    continue;
                pathList.Add(file);
            }
        }
        private static void UpdateModelElement(string path)
        {
            var obj = PrefabUtility.LoadPrefabContents(path);
            var model = obj.GetComponent<Model>();
            if (!model)
                return;
            model.RendererGroup.UpdateRendererElements();
            PrefabUtility.SaveAsPrefabAsset(obj, path, out var successfully);
            PrefabUtility.UnloadPrefabContents(obj);
            Debug.Log($"{path}: {successfully}");
        }
    }
}
