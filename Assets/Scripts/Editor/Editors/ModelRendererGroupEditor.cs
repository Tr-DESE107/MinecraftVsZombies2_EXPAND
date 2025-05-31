using MVZ2.Models;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    [CustomEditor(typeof(ModelRendererGroup))]
    public class ModelRendererGroupEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            var group = target as ModelRendererGroup;
            var testModeProperty = serializedObject.FindProperty("testMode");
            var sortingGroupProperty = serializedObject.FindProperty("sortingGroup");
            var subSortingGroupsProperty = serializedObject.FindProperty("subSortingGroups");
            var renderersProperty = serializedObject.FindProperty("renderers");
            var transformsProperty = serializedObject.FindProperty("transforms");
            var particlesProperty = serializedObject.FindProperty("particles");
            var animatorsProperty = serializedObject.FindProperty("animators");
            EditorGUILayout.PropertyField(testModeProperty);
            EditorGUILayout.PropertyField(sortingGroupProperty);
            EditorGUILayout.PropertyField(subSortingGroupsProperty);
            EditorGUILayout.PropertyField(renderersProperty);
            EditorGUILayout.PropertyField(transformsProperty);
            EditorGUILayout.PropertyField(particlesProperty);
            EditorGUILayout.PropertyField(animatorsProperty);
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateElements();
                EditorUtility.SetDirty(group);
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
