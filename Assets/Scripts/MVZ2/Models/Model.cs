using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Effects;
using MVZ2.Managers;
using PVZEngine;
using PVZEngine.Models;
using Tools;
using UnityEditor;
using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class Model : MonoBehaviour
    {
        #region 公有方法
        public void Init()
        {
            Init(Guid.NewGuid().GetHashCode());
        }
        public void Init(int seed)
        {
            modelComponents = GetComponents<ModelComponent>();
            rng = new RandomGenerator(seed);
            foreach (var comp in modelComponents)
            {
                comp.Model = this;
                comp.Init();
            }
            modelInterface = new ModelParentInterface(this);
        }
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
                id = ID,
                key = Key,
                anchor = AnchorName,
                rng = rng.ToSerializable(),
                propertyDict = propertyDict != null ? propertyDict.Serialize() : null,
                rendererGroup = rendererGroup.ToSerializable(),
                childModels = childModels.Select(c => c.ToSerializable()).ToArray(),
                triggeredEvents = triggeredEvents.ToArray(),
                triggeringEvents = triggeringEvents.ToArray()
            };
        }
        public void LoadFromSerializable(SerializableModelData serializable)
        {
            rng = RandomGenerator.FromSerializable(serializable.rng);
            foreach (var seriChild in serializable.childModels)
            {
                var child = CreateChildModel(seriChild.anchor, seriChild.key, seriChild.id);
                child.LoadFromSerializable(seriChild);
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
        public Model CreateChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            var existing = GetChildModel(key);
            if (existing)
                RemoveChildModel(key);
            var anchor = GetAnchor(anchorName);
            if (!anchor)
                return null;
            var child = Model.Create(modelID, anchor.transform);
            if (!child)
                return null;
            child.transform.localPosition = Vector3.zero;
            child.AnchorName = anchorName;
            child.key = key;
            child.parent = this;
            childModels.Add(child);
            return child;
        }
        public bool RemoveChildModel(NamespaceID key)
        {
            var model = childModels.FirstOrDefault(m => m.Key == key);
            if (!model)
                return false;
            Destroy(model.gameObject);
            return childModels.Remove(model);
        }
        public Model GetChildModel(NamespaceID key)
        {
            var model = childModels.FirstOrDefault(m => m.Key == key);
            if (!model)
                return null;
            return model;
        }
        public void ChangeChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            RemoveChildModel(key);
            CreateChildModel(anchorName, key, modelID);
        }
        public IModelInterface GetParentModelInterface()
        {
            return modelInterface;
        }

        public ModelAnchor GetAnchor(string name)
        {
            return modelAnchors.FirstOrDefault(a => a.key == name);
        }

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

        public static Model GetPrefab(NamespaceID modelID)
        {
            var main = MainManager.Instance;
            var res = main.ResourceManager;
            var modelMeta = res.GetModelMeta(modelID);
            if (modelMeta == null)
                return null;
            return res.GetModel(modelMeta.Path);
        }
        public static Model Create(NamespaceID modelID, Transform parent)
        {
            var prefab = GetPrefab(modelID);
            var model = Create(prefab, parent);
            if (model)
                model.id = modelID;
            return model;
        }
        public static Model Create(NamespaceID modelID, Transform parent, int seed)
        {
            var prefab = GetPrefab(modelID);
            var model = Create(prefab, parent, seed);
            if (model)
                model.id = modelID;
            return model;
        }
        public static Model Create(Model prefab, Transform parent)
        {
            if (prefab == null)
                return null;
            var model = Instantiate(prefab, parent).GetComponent<Model>();
            model.Init();
            return model;
        }
        public static Model Create(Model prefab, Transform parent, int seed)
        {
            if (prefab == null)
                return null;
            var model = Instantiate(prefab, parent).GetComponent<Model>();
            model.Init(seed);
            return model;
        }

        #endregion

        #region 私有方法
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
        public float AnimationSpeed { get; set; }
        public string AnchorName { get; private set; }
        public NamespaceID ID => id;
        public NamespaceID Key => key;
        public Model Parent => parent;
        [SerializeField]
        private NamespaceID id;
        [SerializeField]
        private NamespaceID key;
        [SerializeField]
        private Model parent;
        [SerializeField]
        private MultipleRendererGroup rendererGroup;
        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private Transform centerTransform;
        [SerializeField]
        private ModelAnchor[] modelAnchors;
        [SerializeField]
        private ModelComponent[] modelComponents;
        [SerializeField]
        private List<Model> childModels = new List<Model>();
        private ModelParentInterface modelInterface;
        private RandomGenerator rng;
        private List<string> triggeringEvents = new List<string>();
        private List<string> triggeredEvents = new List<string>();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        #endregion
    }
    public class SerializableModelData
    {
        public string anchor;
        public NamespaceID key;
        public NamespaceID id;
        public SerializableRNG rng;
        public SerializableMultipleRendererGroup rendererGroup;
        public SerializablePropertyDictionary propertyDict;
        public SerializableModelData[] childModels;
        public string[] triggeringEvents;
        public string[] triggeredEvents;
    }
}