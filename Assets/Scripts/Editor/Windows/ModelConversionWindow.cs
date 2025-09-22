using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    public class ModelConversionWindow : EditorWindow
    {
        ConversionData conversion;
        MoveColliderData moveCollider;
        Vector2 scrollPosition;
        SerializedObject seriConversion;
        SerializedObject seriMoveCollider;
        public static void ShowWindow()
        {
            GetWindow<ModelConversionWindow>("Model Conversion");
        }
        private void OnEnable()
        {
            if (!conversion)
            {
                conversion = ScriptableObject.CreateInstance<ConversionData>();
            }
            if (!moveCollider)
            {
                moveCollider = ScriptableObject.CreateInstance<MoveColliderData>();
            }
            seriConversion = new SerializedObject(conversion);
            seriMoveCollider = new SerializedObject(moveCollider);
        }

        void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Model Conversion", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 选择Prefab
            EditorGUILayout.PropertyField(seriConversion.FindProperty("selectedPrefabsVariant"), new GUIContent("Variant Prefabs"));
            EditorGUILayout.PropertyField(seriConversion.FindProperty("selectedPrefabBase"), new GUIContent("Relative To"));
            EditorGUILayout.PropertyField(seriConversion.FindProperty("selectedPrefabNewBase"), new GUIContent("New Base"));
            EditorGUILayout.PropertyField(seriConversion.FindProperty("selectedPrefabBoneBase"), new GUIContent("Bone Base"));
            seriConversion.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (GUILayout.Button("Convert"))
            {
                if (conversion != null && conversion.selectedPrefabsVariant.Count > 0 && conversion.selectedPrefabBase && conversion.selectedPrefabNewBase && conversion.selectedPrefabBoneBase)
                {
                    var basePath = AssetDatabase.GetAssetPath(conversion.selectedPrefabBase);
                    var newBasePath = AssetDatabase.GetAssetPath(conversion.selectedPrefabNewBase);
                    var boneBasePath = AssetDatabase.GetAssetPath(conversion.selectedPrefabBoneBase);
                    foreach (var variant in conversion.selectedPrefabsVariant)
                    {
                        var variantPath = AssetDatabase.GetAssetPath(variant);
                        ModelMenu.ConvertModelToBase(variantPath, basePath, newBasePath, boneBasePath);
                    }
                }
            }


            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Move Model Collider", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 选择Prefab
            EditorGUILayout.PropertyField(seriMoveCollider.FindProperty("selectedPrefabsVariant"), new GUIContent("Models"));
            EditorGUILayout.PropertyField(seriMoveCollider.FindProperty("selectedColliderPrefab"), new GUIContent("Collider Prefab"));
            seriMoveCollider.ApplyModifiedProperties();

            EditorGUILayout.Space();
            if (GUILayout.Button("Move"))
            {
                if (moveCollider != null && moveCollider.selectedPrefabsVariant.Count > 0)
                {
                    foreach (var variant in moveCollider.selectedPrefabsVariant)
                    {
                        var variantPath = AssetDatabase.GetAssetPath(variant);
                        ModelMenu.MoveModelColliderToRoot(variantPath, moveCollider.selectedColliderPrefab);
                    }
                }
            }

            EditorGUILayout.EndScrollView();
        }
        [Serializable]
        private class ConversionData : ScriptableObject
        {
            public List<GameObject> selectedPrefabsVariant = new List<GameObject>();
            public GameObject selectedPrefabBase;
            public GameObject selectedPrefabNewBase;
            public GameObject selectedPrefabBoneBase;
        }
        [Serializable]
        private class MoveColliderData : ScriptableObject
        {
            public List<GameObject> selectedPrefabsVariant = new List<GameObject>();
            public GameObject selectedColliderPrefab;
        }
    }
}