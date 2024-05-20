using System.IO;
using System.Linq;
using PVZEngine;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(ResourceManager))]
    public class ResourceManagerEditor : UnityEditor.Editor
    {
        private ResourceManager resourceManager;
        private void OnEnable()
        {
            resourceManager = target as ResourceManager;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Model References"))
            {
                GenerateModelReferences();
            }
            if (GUILayout.Button("Generate Sound References"))
            {
                GenerateSoundReferences();
            }
            if (GUILayout.Button("Generate All References"))
            {
                GenerateModelReferences();
                GenerateSoundReferences();
            }
        }
        private void GenerateModelReferences()
        {
            var directory = resourceManager.EditorModelDirectory;
            var assets = AssetDatabase.FindAssets("t:Prefab", new string[] { directory });
            var audioItems = assets.Select(hash =>
            {
                var path = AssetDatabase.GUIDToAssetPath(hash);
                if (path.Replace('\\', '/').Contains("/_"))
                    return null;
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (!go)
                    return null;
                var model = go.GetComponent<Model>();
                if (!model)
                    return null;
                var nsp = "mvz2";
                var name = NamespaceID.ConvertName(Path.GetFileNameWithoutExtension(path));
                return new ModelResource()
                {
                    id = new NamespaceID(nsp, name),
                    resource = model
                };
            }).Where(i => i != null);
            resourceManager.SetModelResources(audioItems.ToArray());
        }
        private void GenerateSoundReferences()
        {
            var directory = resourceManager.EditorSoundDirectory;
            var assets = AssetDatabase.FindAssets("t:AudioResource", new string[] { directory });
            var audioItems = assets.Select(hash =>
            {
                var path = AssetDatabase.GUIDToAssetPath(hash);
                if (path.Replace('\\', '/').Contains("/__"))
                    return null;
                var res = AssetDatabase.LoadAssetAtPath<AudioResource>(path);
                if (res == null)
                    return null;
                var nsp = "mvz2";
                var name = NamespaceID.ConvertName(Path.GetFileNameWithoutExtension(path));
                res.id = new NamespaceID(nsp, name);
                return res;
            }).Where(i => i != null);
            resourceManager.SetAudioResources(audioItems.ToArray());
        }
    }
}
