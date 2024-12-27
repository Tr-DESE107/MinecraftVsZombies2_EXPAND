using System;
using System.Linq;
using MVZ2.Models;
using UnityEngine;

namespace MVZ2.Level
{
    public class AreaModel : MonoBehaviour
    {
        public void UpdateFrame(float deltaTime)
        {
            updateGroup.UpdateFrame(deltaTime);
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            updateGroup.SetSimulationSpeed(simulationSpeed);
        }
        public void SetDarknessValue(float value)
        {
            foreach (var renderer in darknessRenderers)
            {
                renderer.color = new Color(0, 0, 0, value);
            }
        }
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
        public void SetDoorVisible(bool visible)
        {
            foreach (var obj in doorObjects)
            {
                obj.SetActive(visible);
            }
        }
        #region 序列化
        public SerializableAreaModelData ToSerializable()
        {
            return new SerializableAreaModelData()
            {
                currentPreset = currentPreset,
                updateGroup = updateGroup.ToSerializable(),
            };
        }
        public void LoadFromSerializable(SerializableAreaModelData serializable)
        {
            SetPreset(serializable.currentPreset);
            updateGroup.LoadFromSerializable(serializable.updateGroup);
        }
        #endregion

        private string currentPreset;

        [SerializeField]
        private ModelUpdateGroup updateGroup;
        [SerializeField]
        private GameObject[] doorObjects;
        [SerializeField]
        private SpriteRenderer[] darknessRenderers;
        [SerializeField]
        private AreaModelPreset[] presets;
    }
    [Serializable]
    public class SerializableAreaModelData
    {
        public string currentPreset;
        public SerializableModelUpdateGroup updateGroup;
    }
}
