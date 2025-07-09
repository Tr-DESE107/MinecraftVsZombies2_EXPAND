﻿using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Models;
using UnityEngine;

namespace MVZ2.Level
{
    [DisallowMultipleComponent]
    public class AreaModel : Model
    {
        public void SetPreset(string name)
        {
            bool hasActive = false;
            foreach (var preset in presets)
            {
                bool active = preset.GetName() == name;
                if (active)
                {
                    hasActive = true;
                }
                preset.SetActive(active);
            }
            if (!hasActive)
            {
                var preset = presets.FirstOrDefault();
                if (preset)
                {
                    preset.SetActive(true);
                }
            }
            currentPreset = name;
        }
        public override void UpdateElements()
        {
            base.UpdateElements();
            var newGroup = GetComponent<ModelGroupArea>();
            if (newGroup != group)
            {
                group = newGroup;
            }
            var newPresets = GetComponentsInChildren<AreaModelPreset>(true)
                .Where(g => g.IsDirectChild<Model>(this) && g.gameObject != gameObject);
            presets.ReplaceList(newPresets);
        }
        #region 序列化
        protected override SerializableModelData CreateSerializable()
        {
            var serializable = new SerializableAreaModelData();
            serializable.currentPreset = currentPreset;
            return serializable;
        }
        protected override void LoadSerializable(SerializableModelData serializable)
        {
            base.LoadSerializable(serializable);
            if (serializable is not SerializableAreaModelData areaModel)
                return;
            SetPreset(areaModel.currentPreset);
        }
        #endregion

        public override ModelGroup GraphicGroup => RendererGroup;
        public ModelGroupArea RendererGroup => group;
        private string currentPreset;
        [Header("Area")]
        [SerializeField]
        private ModelGroupArea group;
        [SerializeField]
        private List<AreaModelPreset> presets = new List<AreaModelPreset>();
    }
    [Serializable]
    public class SerializableAreaModelData : SerializableModelData
    {
        public string currentPreset;
        [Obsolete]
        public SerializableModelUpdateGroup updateGroup;
    }
}
