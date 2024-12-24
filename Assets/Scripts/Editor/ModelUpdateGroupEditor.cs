using MVZ2.Models;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(ModelUpdateGroup))]
    public class ModelUpdateGroupEditor : UnityEditor.Editor
    {
        private ModelUpdateGroup group;
        private SerializedProperty particlesProperty;
        private SerializedProperty animatorsProperty;
        private void OnEnable()
        {
            group = target as ModelUpdateGroup;
            particlesProperty = serializedObject.FindProperty("particles");
            animatorsProperty = serializedObject.FindProperty("animators");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(particlesProperty);
            EditorGUILayout.PropertyField(animatorsProperty);
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateElements();
                EditorUtility.SetDirty(group);
            }
        }
    }
}
