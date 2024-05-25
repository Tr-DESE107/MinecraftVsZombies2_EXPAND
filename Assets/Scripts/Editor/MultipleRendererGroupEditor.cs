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
        private void OnEnable()
        {
            group = target as MultipleRendererGroup;
            sortingGroupProperty = serializedObject.FindProperty("sortingGroup");
            elementsProperty = serializedObject.FindProperty("elements");
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.PropertyField(sortingGroupProperty);
            EditorGUILayout.PropertyField(elementsProperty);
            if (GUILayout.Button("Update Elements"))
            {
                group.UpdateRendererElements();
            }
        }
    }
}
