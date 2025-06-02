using System.IO;
using MVZ2Logic;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    public class SaveDataMenu
    {
        public static void DecompressSaveData()
        {
            var directory = Path.Combine(Application.persistentDataPath, "userdata");
            var filePath = EditorUtility.OpenFilePanel("Open compressed save data file", directory, "dat,lvl");
            if (!File.Exists(filePath))
                return;
            var json = SerializeHelper.ReadCompressedJson(filePath);
            var destPath = EditorUtility.SaveFilePanel("Save decompressed save data file", directory, "Decompressed", "json");
            SerializeHelper.WriteJson(destPath, json);
            Debug.Log($"Save data decompressed to {destPath}.");
        }
        public static void CompressSaveData()
        {
            var directory = Path.Combine(Application.persistentDataPath, "userdata");
            var filePath = EditorUtility.OpenFilePanel("Open decompressed save data file", directory, "");
            if (!File.Exists(filePath))
                return;
            var json = SerializeHelper.ReadJson(filePath);
            var destPath = EditorUtility.SaveFilePanel("Save compressed save data file", directory, "Compressed", "dat");
            SerializeHelper.WriteCompressedJson(destPath, json);
            Debug.Log($"Save data compressed to {destPath}.");
        }
    }
}
