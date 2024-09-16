using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using PVZEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MVZ2.Editor
{
    public class LanguagePackMenu : MonoBehaviour
    {
        [MenuItem("Custom/Language/Build Builtin Pack")]
        public static void BuildBuiltinPack()
        {
            var packRoot = Path.Combine(Application.dataPath, "..", "ExternalData", "languages");
            var directoryPath = Path.Combine(packRoot, "builtin");
            var finalFilePath = Path.Combine(packRoot, "builtin.pack");

            LanguageManager.CompressLanguagePack(directoryPath, finalFilePath);
            Debug.Log("Successfully built the builtin language pack.");
        }
    }
}
