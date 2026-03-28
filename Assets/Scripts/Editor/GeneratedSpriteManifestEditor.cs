using System.IO;
using MVZ2.Sprites;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(GeneratedSpriteManifest))]  // 使用 CustomEditor 而不是 CreateAssetMenu  
    public class GeneratedSpriteManifestEditor : UnityEditor.Editor  // 继承 Editor 而不是 ScriptableObject  
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认Inspector    
            DrawDefaultInspector();

            EditorGUILayout.Space();

            // 添加"Export All Sprites"按钮    
            if (GUILayout.Button("Export All Sprites", GUILayout.Height(30)))
            {
                ExportAllSprites();
            }
        }

        private void ExportAllSprites()
        {
            var manifest = (GeneratedSpriteManifest)target;

            // 选择导出目录    
            string folderPath = EditorUtility.OpenFolderPanel(
                "Select Export Folder",
                Application.dataPath,
                ""
            );

            if (string.IsNullOrEmpty(folderPath))
                return;

            int exportedCount = 0;

            // 使用反射访问私有字段 categories    
            var categoriesField = typeof(GeneratedSpriteManifest)
                .GetField("categories", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var categories = categoriesField.GetValue(manifest) as System.Collections.IList;

            if (categories == null || categories.Count == 0)
            {
                EditorUtility.DisplayDialog("Export Failed", "No sprites to export.", "OK");
                return;
            }

            // 遍历所有分类    
            foreach (var categoryObj in categories)
            {
                var categoryType = categoryObj.GetType();
                var categoryName = categoryType.GetField("name").GetValue(categoryObj) as string;

                // 创建分类子目录    
                string categoryPath = Path.Combine(folderPath, SanitizeFileName(categoryName));
                Directory.CreateDirectory(categoryPath);

                // 获取该分类下的所有精灵图    
                var spritesField = categoryType.GetField("sprites", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var sprites = spritesField.GetValue(categoryObj) as System.Collections.IList;

                if (sprites == null)
                    continue;

                // 导出每个精灵图    
                foreach (var spritePreviewObj in sprites)
                {
                    var previewType = spritePreviewObj.GetType();
                    var sprite = previewType.GetField("sprite").GetValue(spritePreviewObj) as Sprite;
                    var name = previewType.GetField("name").GetValue(spritePreviewObj) as string;

                    if (sprite == null || string.IsNullOrEmpty(name))
                        continue;

                    // 导出PNG    
                    string fileName = SanitizeFileName(name) + ".png";
                    string filePath = Path.Combine(categoryPath, fileName);

                    var bytes = sprite.texture.EncodeToPNG();
                    File.WriteAllBytes(filePath, bytes);

                    exportedCount++;
                }
            }

            EditorUtility.DisplayDialog(
                "Export Complete",
                $"Successfully exported {exportedCount} sprites to:\n{folderPath}",
                "OK"
            );

            // 在文件浏览器中打开导出目录    
            EditorUtility.RevealInFinder(folderPath);
        }

        private string SanitizeFileName(string name)
        {
            foreach (var chr in Path.GetInvalidFileNameChars())
            {
                name = name.Replace(chr, '_');
            }
            return name;
        }
    }
}
