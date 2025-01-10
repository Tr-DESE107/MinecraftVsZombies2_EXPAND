using MVZ2.Models;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(ModelRendererGroup))]
    public class ModelRendererGroupEditor : UnityEditor.Editor
    {
        private ModelRendererGroup group;
        private SerializedProperty sortingGroupProperty;
        private SerializedProperty renderersProperty;
        private SerializedProperty transformsProperty;
        private SerializedProperty particlesProperty;
        private SerializedProperty animatorsProperty;
        private void OnEnable()
        {
            group = target as ModelRendererGroup;
            sortingGroupProperty = serializedObject.FindProperty("sortingGroup");
            renderersProperty = serializedObject.FindProperty("renderers");
            transformsProperty = serializedObject.FindProperty("transforms");
            particlesProperty = serializedObject.FindProperty("particles");
            animatorsProperty = serializedObject.FindProperty("animators");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(sortingGroupProperty);
            EditorGUILayout.PropertyField(renderersProperty);
            EditorGUILayout.PropertyField(transformsProperty);
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
