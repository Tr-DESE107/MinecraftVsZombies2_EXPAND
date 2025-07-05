using MVZ2.Models;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(Model), editorForChildClasses: true)]
    public class ModelEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var group = target as Model;
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateElements();
                EditorUtility.SetDirty(group);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
