using System.IO;
using System.Linq;
using MVZ2.Rendering;
using PVZEngine;
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
        private void OnEnable()
        {
            group = target as MultipleRendererGroup;
            sortingGroupProperty = serializedObject.FindProperty("sortingGroup");
            elementsProperty = serializedObject.FindProperty("elements");
            particlesProperty = serializedObject.FindProperty("particles");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(sortingGroupProperty);
            EditorGUILayout.PropertyField(elementsProperty);
            EditorGUILayout.PropertyField(particlesProperty);
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateRendererElements();
            }
        }
    }
}
