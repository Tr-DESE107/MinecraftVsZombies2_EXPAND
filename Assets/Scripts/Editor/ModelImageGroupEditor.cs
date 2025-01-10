using MVZ2.Models;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(ModelImageGroup))]
    public class ModelImageGroupEditor : UnityEditor.Editor
    {
        private ModelImageGroup group;
        private SerializedProperty imagesProperty;
        private SerializedProperty transformsProperty;
        private SerializedProperty animatorsProperty;
        private void OnEnable()
        {
            group = target as ModelImageGroup;
            imagesProperty = serializedObject.FindProperty("images");
            transformsProperty = serializedObject.FindProperty("transforms");
            animatorsProperty = serializedObject.FindProperty("animators");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(imagesProperty);
            EditorGUILayout.PropertyField(transformsProperty);
            EditorGUILayout.PropertyField(animatorsProperty);
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateElements();
                EditorUtility.SetDirty(group);
            }
        }
    }
}
