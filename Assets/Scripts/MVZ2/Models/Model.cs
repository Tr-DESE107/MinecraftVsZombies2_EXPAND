using System;
using System.Collections.Generic;
using PVZEngine;
using Tools;
using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class Model : MonoBehaviour
    {
        #region 公有方法
        public void UpdateFixed()
        {
            triggeringEvents.Clear();

            foreach (var comp in modelComponents)
            {
                comp.UpdateLogic();
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            rendererGroup.UpdateFrame(deltaTime);
            foreach (var comp in modelComponents)
            {
                comp.UpdateFrame(deltaTime);
            }
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            RendererGroup.SetSimulationSpeed(simulationSpeed);
        }
        public void TriggerAnimator(string name)
        {
            RendererGroup.TriggerAnimator(name);
        }
        public void SetAnimatorBool(string name, bool value)
        {
            RendererGroup.SetAnimatorBool(name, value);
        }
        public void SetAnimatorInt(string name, int value)
        {
            RendererGroup.SetAnimatorInt(name, value);
        }
        public void SetAnimatorFloat(string name, float value)
        {
            RendererGroup.SetAnimatorFloat(name, value);
        }
        public bool IsEventTriggered(string name)
        {
            return triggeringEvents.Contains(name);
        }

        public bool WasEventTriggered(string name)
        {
            return triggeredEvents.Contains(name);
        }

        public RandomGenerator GetRNG()
        {
            return rng;
        }

        public SerializableModelData ToSerializable()
        {
            return new SerializableModelData()
            {
                rng = rng.ToSerializable(),
                armorModel = armorModel ? armorModel.ToSerializable() : null,
                propertyDict = propertyDict != null ? propertyDict.Serialize() : null,
                rendererGroup = rendererGroup.ToSerializable(),
                triggeredEvents = triggeredEvents.ToArray(),
                triggeringEvents = triggeringEvents.ToArray()
            };
        }
        public void LoadFromSerializable(SerializableModelData serializable)
        {
            rng = RandomGenerator.FromSerializable(serializable.rng);
            if (armorModel && serializable.armorModel != null)
            {
                armorModel.LoadFromSerializable(serializable.armorModel);
            }
            rendererGroup.LoadFromSerializable(serializable.rendererGroup);
            if (serializable.propertyDict != null)
            {
                var dict = PropertyDictionary.Deserialize(serializable.propertyDict);
                foreach (var name in dict.GetPropertyNames())
                {
                    SetProperty(name, dict.GetProperty(name));
                }
            }
            triggeredEvents.Clear();
            triggeringEvents.Clear();
            triggeredEvents.AddRange(serializable.triggeredEvents);
            triggeringEvents.AddRange(serializable.triggeringEvents);
        }
        #region 护甲
        public void SetArmor(Model model)
        {
            if (ArmorModel)
            {
                RemoveArmor();
            }
            if (!model)
                return;
            if (!armorTransform)
                return;
            model.transform.parent = armorTransform;
            model.transform.localPosition = Vector3.zero;
            armorModel = model;
        }
        public void RemoveArmor()
        {
            if (!armorModel)
                return;
            Destroy(armorModel.gameObject);
            armorModel = null;
        }
        #endregion

        #region 属性
        public T GetProperty<T>(string name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty(string name, object value)
        {
            propertyDict.SetProperty(name, value);
            foreach (var comp in modelComponents)
            {
                comp.OnPropertySet(name, value);
            }
        }
        #endregion

        #region 着色器属性
        public void SetShaderFloat(string name, float value)
        {
            rendererGroup.SetPropertyFloat(name, value);
        }
        public void SetShaderInt(string name, int value)
        {
            rendererGroup.SetPropertyInt(name, value);
        }
        public void SetShaderColor(string name, Color value)
        {
            rendererGroup.SetPropertyColor(name, value);
        }
        #endregion

        #endregion

        #region 私有方法
        private void Awake()
        {
            modelComponents = GetComponents<ModelComponent>();
            rng = new RandomGenerator(Guid.NewGuid().GetHashCode());
            foreach (var comp in modelComponents)
            {
                comp.Model = this;
                comp.Init();
            }
        }
        private void TriggerEvent(string name)
        {
            triggeringEvents.Add(name);
            triggeredEvents.Add(name);
        }
        #endregion

        #region 属性字段
        public MultipleRendererGroup RendererGroup => rendererGroup;
        public Transform RootTransform => rootTransform;
        public Transform CenterTransform => centerTransform;
        public Model ArmorModel => armorModel;
        public float AnimationSpeed { get; set; }
        [SerializeField]
        private MultipleRendererGroup rendererGroup;
        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private Transform centerTransform;
        [SerializeField]
        private Transform armorTransform;
        [SerializeField]
        private Model armorModel;
        [SerializeField]
        private ModelComponent[] modelComponents;
        private RandomGenerator rng;
        private List<string> triggeringEvents = new List<string>();
        private List<string> triggeredEvents = new List<string>();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        #endregion
    }
    public class SerializableModelData
    {
        public SerializableRNG rng;
        public SerializableMultipleRendererGroup rendererGroup;
        public SerializableModelData armorModel;
        public SerializablePropertyDictionary propertyDict;
        public string[] triggeringEvents;
        public string[] triggeredEvents;
    }
}