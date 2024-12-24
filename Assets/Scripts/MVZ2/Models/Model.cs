using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Models;
using Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class Model : MonoBehaviour
    {
        #region 公有方法

        #region 生命周期
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
            foreach (var comp in modelComponents)
            {
                comp.UpdateLogic();
            }
            childModels.RemoveAll(m => !m);
            foreach (var child in childModels)
            {
                child.UpdateFixed();
            }
            if (destroyTimeout > 0)
            {
                destroyTimeout--;
                if (destroyTimeout <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
        public void UpdateFrame(float deltaTime)
        {
            rendererGroup.UpdateFrame(deltaTime);
            foreach (var comp in modelComponents)
            {
                comp.UpdateFrame(deltaTime);
            }
            childModels.RemoveAll(m => !m);
            foreach (var child in childModels)
            {
                child.UpdateFrame(deltaTime);
            }
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            RendererGroup.SetSimulationSpeed(simulationSpeed);
            childModels.RemoveAll(m => !m);
            foreach (var child in childModels)
            {
                child.SetSimulationSpeed(simulationSpeed);
            }
        }
        public void SetLight(bool visible, Vector2 range, Color color, Vector2 randomOffset)
        {
            lightController.gameObject.SetActive(visible);
            lightController.SetColor(color);
            lightController.SetRange(range, randomOffset);
        }
        #endregion

        #region 动画
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
        #endregion

        #region 序列化
        public SerializableModelData ToSerializable()
        {
            return new SerializableModelData()
            {
                id = id,
                key = parentKey,
                anchor = parentAnchor,
                rng = rng.ToSerializable(),
                propertyDict = propertyDict != null ? propertyDict.Serialize() : null,
                rendererGroup = rendererGroup.ToSerializable(),
                childModels = childModels.Select(c => c.ToSerializable()).ToArray(),
                destroyTimeout = destroyTimeout,
                light = lightController.ToSerializable(),
            };
        }
        public void LoadFromSerializable(SerializableModelData serializable)
        {
            rng = RandomGenerator.FromSerializable(serializable.rng);
            rendererGroup.LoadFromSerializable(serializable.rendererGroup);
            lightController.LoadFromSerializable(serializable.light);
            destroyTimeout = serializable.destroyTimeout;
            if (serializable.propertyDict != null)
            {
                var dict = PropertyDictionary.Deserialize(serializable.propertyDict);
                foreach (var name in dict.GetPropertyNames())
                {
                    SetProperty(name, dict.GetProperty(name));
                }
            }
            foreach (var seriChild in serializable.childModels)
            {
                var child = CreateChildModel(seriChild.anchor, seriChild.key, seriChild.id);
                child.LoadFromSerializable(seriChild);
            }
        }
        #endregion

        #region 子模型
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
            child.parentAnchor = anchorName;
            child.parentKey = key;
            child.parent = this;
            childModels.Add(child);
            return child;
        }
        public bool RemoveChildModel(NamespaceID key)
        {
            var model = GetChildModel(key);
            if (!model)
                return false;
            if (model.destroyDelay <= 0)
            {
                Destroy(model.gameObject);
                return childModels.Remove(model);
            }
            else
            {
                model.DestroyDelayed(model.destroyDelay);
                return true;
            }
        }
        public Model GetChildModel(NamespaceID key)
        {
            var model = childModels.FirstOrDefault(m => !m.IsDestroying() && m.parentKey == key);
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
        #endregion

        #region 摧毁
        public void DestroyDelayed(int frames)
        {
            destroyTimeout = frames;
            onDelayedDestroy?.Invoke();
        }
        public bool IsDestroying()
        {
            return destroyTimeout > 0;
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
        public void TriggerModel(string name)
        {
            foreach (var comp in modelComponents)
            {
                comp.OnTrigger(name);
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

        #region 创建
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

        public static ModelMeta GetModelMeta(NamespaceID modelID)
        {
            var main = MainManager.Instance;
            var res = main.ResourceManager;
            return res.GetModelMeta(modelID);
        }
        public static Model GetPrefab(NamespaceID modelID)
        {
            var main = MainManager.Instance;
            var res = main.ResourceManager;
            var modelMeta = GetModelMeta(modelID);
            if (modelMeta == null)
                return null;
            return res.GetModel(modelMeta.Path);
        }
        public ModelAnchor GetAnchor(string name)
        {
            return modelAnchors.FirstOrDefault(a => a.key == name);
        }
        public RandomGenerator GetRNG()
        {
            return rng;
        }
        #endregion

        #region 属性字段
        public MultipleRendererGroup RendererGroup => rendererGroup;
        public Transform GetRootTransform() => GetAnchor(LogicModelHelper.ANCHOR_ROOT).transform;
        public Transform GetCenterTransform() => GetAnchor(LogicModelHelper.ANCHOR_CENTER).transform;

        private NamespaceID id;
        private NamespaceID parentKey;
        private string parentAnchor;

        private int destroyTimeout;
        private ModelParentInterface modelInterface;
        private RandomGenerator rng;
        private PropertyDictionary propertyDict = new PropertyDictionary();

        [Header("General")]
        [SerializeField]
        private MultipleRendererGroup rendererGroup;
        [SerializeField]
        private LightController lightController;
        [SerializeField]
        private ModelAnchor[] modelAnchors;
        [SerializeField]
        private ModelComponent[] modelComponents;

        [Header("Nesting")]
        [SerializeField]
        private Model parent;
        [SerializeField]
        private List<Model> childModels = new List<Model>();

        [Header("Destruction")]
        [SerializeField]
        private int destroyDelay;
        [SerializeField]
        private UnityEvent onDelayedDestroy;
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
        public SerializableLightController light;
        public int destroyTimeout;
    }
}