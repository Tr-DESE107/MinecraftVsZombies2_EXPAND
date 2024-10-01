using MVZ2.Rendering;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(MultipleRendererGroup))]
    public class MultipleRendererGroupEditor : UnityEditor.Editor
    {
        private MultipleRendererGroup group;
        private SerializedProperty sortingGroupProperty;
        private SerializedProperty elementsProperty;
        private SerializedProperty particlesProperty;
        private SerializedProperty lightControllerProperty;
        private SerializedProperty animatorsProperty;
        private void OnEnable()
        {
            group = target as MultipleRendererGroup;
            sortingGroupProperty = serializedObject.FindProperty("sortingGroup");
            elementsProperty = serializedObject.FindProperty("elements");
            particlesProperty = serializedObject.FindProperty("particles");
            lightControllerProperty = serializedObject.FindProperty("lightController");
            animatorsProperty = serializedObject.FindProperty("animators");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(sortingGroupProperty);
            EditorGUILayout.PropertyField(lightControllerProperty);
            EditorGUILayout.PropertyField(elementsProperty);
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
