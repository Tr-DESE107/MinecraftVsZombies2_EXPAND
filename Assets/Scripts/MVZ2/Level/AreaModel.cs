using System;
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

        public override ModelGraphicGroup GraphicGroup => RendererGroup;
        public ModelRendererGroup RendererGroup => rendererGroup;
        private string currentPreset;
        [Header("Area")]
        [SerializeField]
        private ModelRendererGroup rendererGroup;
        [SerializeField]
        private AreaModelPreset[] presets;
    }
    [Serializable]
    public class SerializableAreaModelData : SerializableModelData
    {
        public string currentPreset;
        [Obsolete]
        public SerializableModelUpdateGroup updateGroup;
    }
}
