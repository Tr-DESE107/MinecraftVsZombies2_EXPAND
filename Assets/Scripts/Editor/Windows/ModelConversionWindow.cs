using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace MVZ2.Editor
{
    public class ModelConversionWindow : EditorWindow
    {
        WindowData data;
        Vector2 scrollPosition;
        SerializedObject seriObj;
        public static void ShowWindow()
        {
            GetWindow<ModelConversionWindow>("Model Conversion");
        }
        private void OnEnable()
        {
            if (!data)
            {
                data = ScriptableObject.CreateInstance<WindowData>();
            }
            seriObj = new SerializedObject(data);
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Model Conversion", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            // 选择Prefab
            EditorGUILayout.PropertyField(seriObj.FindProperty("selectedPrefabsVariant"), new GUIContent("Variant Prefabs"));
            EditorGUILayout.PropertyField(seriObj.FindProperty("selectedPrefabBase"), new GUIContent("Relative To"));
            EditorGUILayout.PropertyField(seriObj.FindProperty("selectedPrefabNewBase"), new GUIContent("New Base"));
            EditorGUILayout.PropertyField(seriObj.FindProperty("selectedPrefabBoneBase"), new GUIContent("Bone Base"));
            seriObj.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (GUILayout.Button("Convert"))
            {
                if (data != null && data.selectedPrefabsVariant.Count > 0 && data.selectedPrefabBase && data.selectedPrefabNewBase && data.selectedPrefabBoneBase)
                {
                    var basePath = AssetDatabase.GetAssetPath(data.selectedPrefabBase);
                    var newBasePath = AssetDatabase.GetAssetPath(data.selectedPrefabNewBase);
                    var boneBasePath = AssetDatabase.GetAssetPath(data.selectedPrefabBoneBase);
                    foreach (var variant in data.selectedPrefabsVariant)
                    {
                        var variantPath = AssetDatabase.GetAssetPath(variant);
                        ModelMenu.ConvertModelToBase(variantPath, basePath, newBasePath, boneBasePath);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
        [Serializable]
        private class WindowData : ScriptableObject
        {
            public List<GameObject> selectedPrefabsVariant = new List<GameObject>();
            public GameObject selectedPrefabBase;
            public GameObject selectedPrefabNewBase;
            public GameObject selectedPrefabBoneBase;
        }
    }
}