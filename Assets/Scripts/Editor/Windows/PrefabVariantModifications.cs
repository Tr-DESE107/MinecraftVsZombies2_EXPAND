using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Editor
{
    public class PrefabVariantModifications : EditorWindow
    {
        private GameObject selectedPrefab;
        private Vector2 scrollPosition;
        private Dictionary<string, List<ModificationInfo>> modifications = new Dictionary<string, List<ModificationInfo>>();

        [MenuItem("Tools/Prefab Variant Modifications")]
        public static void ShowWindow()
        {
            GetWindow<PrefabVariantModifications>("Prefab Modifications");
        }

        void OnGUI()
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Prefab Variant Modifications", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 选择Prefab
            EditorGUI.BeginChangeCheck();
            selectedPrefab = (GameObject)EditorGUILayout.ObjectField("Prefab Variant", selectedPrefab, typeof(GameObject), false);
            if (EditorGUI.EndChangeCheck() && selectedPrefab != null)
            {
                AnalyzePrefabVariant();
            }

            EditorGUILayout.Space();

            if (modifications.Count == 0)
            {
                EditorGUILayout.HelpBox("Select a Prefab Variant to analyze its modifications", MessageType.Info);
                return;
            }

            // 显示修改详情
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            foreach (var category in modifications)
            {
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(category.Key, EditorStyles.boldLabel);

                foreach (var mod in category.Value)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.LabelField($"Object: {mod.ObjectName}", EditorStyles.boldLabel);

                    if (!string.IsNullOrEmpty(mod.ComponentType))
                    {
                        EditorGUILayout.LabelField($"Component: {mod.ComponentType}");
                    }

                    if (!string.IsNullOrEmpty(mod.PropertyPath))
                    {
                        EditorGUILayout.LabelField($"Property: {mod.PropertyPath}");
                    }

                    if (!string.IsNullOrEmpty(mod.ModificationType))
                    {
                        EditorGUILayout.LabelField($"Type: {mod.ModificationType}");
                    }

                    if (!string.IsNullOrEmpty(mod.Value))
                    {
                        EditorGUILayout.LabelField($"Value: {mod.Value}");
                    }

                    if (!string.IsNullOrEmpty(mod.ParentValue))
                    {
                        EditorGUILayout.LabelField($"Parent Value: {mod.ParentValue}");
                    }

                    EditorGUILayout.EndVertical();
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void AnalyzePrefabVariant()
        {
            modifications.Clear();

            if (selectedPrefab == null || PrefabUtility.GetPrefabAssetType(selectedPrefab) != PrefabAssetType.Variant)
            {
                return;
            }

            // 获取属性修改
            PropertyModification[] propertyMods = PrefabUtility.GetPropertyModifications(selectedPrefab);
            if (propertyMods != null)
            {
                foreach (var mod in propertyMods)
                {
                    if (mod.target == null)
                        continue;

                    string componentType = mod.target.GetType().Name;
                    string objName = mod.target.name;

                    if (mod.target is Transform trans && !trans.parent)
                        continue;
                    if (mod.target is GameObject go && mod.propertyPath == "m_Name")
                        continue;
                    // 跳过内部属性
                    //if (mod.propertyPath.StartsWith("m_") ||
                    //    mod.propertyPath.StartsWith("unity_") ||
                    //    mod.propertyPath.StartsWith("k__"))
                    //{
                    //    continue;
                    //}

                    var modInfo = new ModificationInfo
                    {
                        ObjectName = objName,
                        ComponentType = componentType,
                        PropertyPath = mod.propertyPath,
                        Value = mod.value,
                        ModificationType = "Property Override"
                    };

                    AddModification("Property Overrides", modInfo);
                }
            }

            // 获取添加的组件
            var addedComponents = PrefabUtility.GetAddedComponents(selectedPrefab);
            foreach (var comp in addedComponents)
            {
                string componentType = comp.instanceComponent.GetType().Name;
                string objName = comp.instanceComponent.gameObject.name;

                var modInfo = new ModificationInfo
                {
                    ObjectName = objName,
                    ComponentType = componentType,
                    ModificationType = "Component Added"
                };

                AddModification("Component Changes", modInfo);
            }

            // 获取移除的组件
            var removedComponents = PrefabUtility.GetRemovedComponents(selectedPrefab);
            foreach (var comp in removedComponents)
            {
                string componentType = comp.assetComponent.GetType().Name;
                string objName = comp.containingInstanceGameObject.name;

                var modInfo = new ModificationInfo
                {
                    ObjectName = objName,
                    ComponentType = componentType,
                    ModificationType = "Component Removed"
                };

                AddModification("Component Changes", modInfo);
            }

            // 获取添加的子对象
            var addedGameObjects = PrefabUtility.GetAddedGameObjects(selectedPrefab);
            foreach (var go in addedGameObjects)
            {
                string objName = go.instanceGameObject.name;

                var modInfo = new ModificationInfo
                {
                    ObjectName = objName,
                    ModificationType = "GameObject Added"
                };

                AddModification("Hierarchy Changes", modInfo);
            }
        }

        private void AddModification(string category, ModificationInfo modInfo)
        {
            if (!modifications.ContainsKey(category))
            {
                modifications[category] = new List<ModificationInfo>();
            }

            modifications[category].Add(modInfo);
        }

        private void OnSelectionChange()
        {
            GameObject selected = Selection.activeGameObject;
            if (selected != null && PrefabUtility.GetPrefabAssetType(selected) == PrefabAssetType.Variant)
            {
                selectedPrefab = selected;
                AnalyzePrefabVariant();
                Repaint();
            }
        }

        private class ModificationInfo
        {
            public string ObjectName;
            public string ComponentType;
            public string PropertyPath;
            public string Value;
            public string ParentValue;
            public string ModificationType;
        }
    }
}