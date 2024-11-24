using MVZ2.Models;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(MultipleRendererGroup))]
    public class MultipleRendererGroupEditor : UnityEditor.Editor
    {
        private MultipleRendererGroup group;
        private SerializedProperty sortingGroupProperty;
        private SerializedProperty renderersProperty;
        private SerializedProperty transformsProperty;
        private SerializedProperty particlesProperty;
        private SerializedProperty lightControllerProperty;
        private SerializedProperty animatorsProperty;
        private void OnEnable()
        {
            group = target as MultipleRendererGroup;
            sortingGroupProperty = serializedObject.FindProperty("sortingGroup");
            lightControllerProperty = serializedObject.FindProperty("lightController");
            renderersProperty = serializedObject.FindProperty("renderers");
            transformsProperty = serializedObject.FindProperty("transforms");
            particlesProperty = serializedObject.FindProperty("particles");
            animatorsProperty = serializedObject.FindProperty("animators");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(sortingGroupProperty);
            EditorGUILayout.PropertyField(lightControllerProperty);
            EditorGUILayout.PropertyField(renderersProperty);
            EditorGUILayout.PropertyField(transformsProperty);
            EditorGUILayout.PropertyField(particlesProperty);
            EditorGUILayout.PropertyField(animatorsProperty);
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateRendererElements();
                EditorUtility.SetDirty(group);
            }
        }
    }
}
