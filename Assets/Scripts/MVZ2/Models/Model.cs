using System;
using System.Collections.Generic;
using System.Linq;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2Logic.Models;
using PVZEngine;
using PVZEngine.Models;
using Tools;
using UnityEngine;
using UnityEngine.Events;

namespace MVZ2.Models
{
    public abstract class Model : MonoBehaviour
    {
        #region 公有方法
        public Camera GetCamera()
        {
            return eventCamera;
        }
        public void AddElement(GraphicElement element)
        {
            GraphicGroup.AddElement(element);
        }
        #region 生命周期
        public virtual void Init(Camera camera, int seed = 0)
        {
            if (seed == 0)
            {
                seed = Guid.NewGuid().GetHashCode();
            }
            eventCamera = camera;
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
            if (!gameObject.activeInHierarchy)
                return;
            GraphicGroup.UpdateFrame(deltaTime);
            foreach (var comp in modelComponents)
            {
                comp.UpdateFrame(deltaTime);
            }
            childModels.RemoveAll(m => !m);
            foreach (var child in childModels)
            {
                child.UpdateFrame(deltaTime);
            }
            OnUpdateFrame?.Invoke(deltaTime);
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            GraphicGroup.SetSimulationSpeed(simulationSpeed);
            childModels.RemoveAll(m => !m);
            foreach (var child in childModels)
            {
                child.SetSimulationSpeed(simulationSpeed);
            }
        }
        public void SetGroundY(float y)
        {
            GraphicGroup.SetGroundY(y);
            childModels.RemoveAll(m => !m);
            foreach (var child in childModels)
            {
                child.SetGroundY(y);
            }
        }
        #endregion

        #region 动画
        public void TriggerAnimator(string name)
        {
            GraphicGroup.TriggerAnimator(name);
        }
        public void SetAnimatorBool(string name, bool value)
        {
            GraphicGroup.SetAnimatorBool(name, value);
        }
        public void SetAnimatorInt(string name, int value)
        {
            GraphicGroup.SetAnimatorInt(name, value);
        }
        public void SetAnimatorFloat(string name, float value)
        {
            GraphicGroup.SetAnimatorFloat(name, value);
        }
        #endregion

        #region 序列化
        public SerializableModelData ToSerializable()
        {
            var serializable = CreateSerializable();
            serializable.id = id;
            serializable.key = parentKey;
            serializable.anchor = parentAnchor;
            serializable.rng = rng.ToSerializable();
            serializable.propertyDict = propertyDict != null ? propertyDict.ToSerializable() : null;
            serializable.childModels = childModels.Select(c => c.ToSerializable()).ToArray();
            serializable.destroyTimeout = destroyTimeout;
            serializable.graphicGroup = GraphicGroup.ToSerializable();
            return serializable;
        }
        public void LoadFromSerializable(SerializableModelData serializable)
        {
            rng = RandomGenerator.FromSerializable(serializable.rng);
            destroyTimeout = serializable.destroyTimeout;
            GraphicGroup.LoadFromSerializable(serializable.graphicGroup);
            if (serializable.propertyDict != null)
            {
                var dict = PropertyDictionaryString.FromSerializable(serializable.propertyDict);
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
            LoadSerializable(serializable);
        }
        protected abstract SerializableModelData CreateSerializable();
        protected virtual void LoadSerializable(SerializableModelData serializable)
        {

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
            var child = Model.Create(modelID, anchor.transform, eventCamera, 0);
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
            foreach (var child in childModels)
            {
                if (!child.IsDestroying() && child.parentKey == key)
                {
                    return child;
                }
            }
            return null;
        }
        public void ChangeChildModel(string anchorName, NamespaceID key, NamespaceID modelID)
        {
            RemoveChildModel(key);
            CreateChildModel(anchorName, key, modelID);
        }
        public void ClearModelAnchor(string anchorName)
        {
            var anchor = GetAnchor(anchorName);
            if (!anchor)
                return;
            var childCount = anchor.transform.childCount;
            for (int i = childCount - 1; i >= 0; i--)
            {
                var child = anchor.transform.GetChild(i);
                Destroy(child.gameObject);
            }
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
        public T GetProperty<T>(PropertyKeyString name)
        {
            return propertyDict.GetProperty<T>(name);
        }
        public void SetProperty(PropertyKeyString name, object value)
        {
            propertyDict.SetProperty(name, value);
            foreach (var comp in modelComponents)
            {
                comp.OnPropertySet(name, value);
            }
        }
        public void TriggerModel(PropertyKeyString name)
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
            GraphicGroup.SetShaderFloat(name, value);
        }
        public void SetShaderInt(string name, int value)
        {
            GraphicGroup.SetShaderInt(name, value);
        }
        public void SetShaderColor(string name, Color value)
        {
            GraphicGroup.SetShaderColor(name, value);
        }
        #endregion

        #region 创建
        public static Model Create(NamespaceID modelID, Transform parent, Camera camera, int seed = 0)
        {
            var prefab = GetPrefab(modelID);
            var model = Create(prefab, parent, camera, seed);
            if (model)
                model.id = modelID;
            return model;
        }
        public static Model Create(Model prefab, Transform parent, Camera camera, int seed = 0)
        {
            if (prefab == null)
                return null;
            var model = Instantiate(prefab, parent).GetComponent<Model>();
            model.Init(camera, seed);
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
            foreach (var anchor in modelAnchors)
            {
                if (anchor == null)
                    continue;
                if (anchor.key == name)
                    return anchor;
            }
            return null;
        }
        public ModelAnchor[] GetAllAnchors()
        {
            return modelAnchors;
        }
        public RandomGenerator GetRNG()
        {
            return rng;
        }
        #endregion

        public event Action<float> OnUpdateFrame;

        #region 属性字段
        public abstract ModelGraphicGroup GraphicGroup { get; }
        public Transform GetRootTransform() => GetAnchor(LogicModelHelper.ANCHOR_ROOT).transform;
        public Transform GetCenterTransform() => GetAnchor(LogicModelHelper.ANCHOR_CENTER).transform;

        private NamespaceID id;
        private NamespaceID parentKey;
        private string parentAnchor;
        private Camera eventCamera;

        private int destroyTimeout;
        private ModelParentInterface modelInterface;
        private RandomGenerator rng;
        private PropertyDictionaryString propertyDict = new PropertyDictionaryString();
        [Header("General")]
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
        public SerializableModelGraphicGroup graphicGroup;
        public SerializablePropertyDictionaryString propertyDict;
        public SerializableModelData[] childModels;
        public int destroyTimeout;
    }
    public class SerializableAnimator
    {
        public SerializableAnimatorPlayingData[] playingDatas;
        public List<string> triggerParameters = new List<string>();
        public Dictionary<string, bool> boolParameters = new Dictionary<string, bool>();
        public Dictionary<string, int> intParameters = new Dictionary<string, int>();
        public Dictionary<string, float> floatParameters = new Dictionary<string, float>();

        public SerializableAnimator(Animator animator)
        {
            int layerCount = animator.layerCount;

            playingDatas = new SerializableAnimatorPlayingData[layerCount];
            for (int i = 0; i < playingDatas.Length; i++)
            {
                AnimatorStateInfo current = animator.GetCurrentAnimatorStateInfo(i);
                AnimatorStateInfo next = animator.GetNextAnimatorStateInfo(i);
                AnimatorTransitionInfo transition = animator.GetAnimatorTransitionInfo(i);

                SerializableAnimatorPlayingData playingData = new SerializableAnimatorPlayingData()
                {
                    currentHash = current.shortNameHash,
                    currentTime = current.normalizedTime,

                    nextHash = next.shortNameHash,
                    nextNormalizedTime = next.normalizedTime,
                    nextLength = next.length == Mathf.Infinity ? 0 : next.length,

                    transitionDuration = transition.duration,
                    transitionDurationUnit = (int)transition.durationUnit,
                    transitionTime = transition.normalizedTime
                };
                playingDatas[i] = playingData;
            }

            foreach (AnimatorControllerParameter para in animator.parameters)
            {
                string name = para.name;
                switch (para.type)
                {
                    case AnimatorControllerParameterType.Bool:
                        boolParameters.Add(name, animator.GetBool(name));
                        break;

                    case AnimatorControllerParameterType.Float:
                        floatParameters.Add(name, (float)animator.GetFloat(name));
                        break;

                    case AnimatorControllerParameterType.Int:
                        intParameters.Add(name, animator.GetInteger(name));
                        break;

                    case AnimatorControllerParameterType.Trigger:
                        if (animator.GetBool(name))
                        {
                            triggerParameters.Add(name);
                        }
                        break;
                }
            }
        }
        public void Deserialize(Animator animator)
        {
            int layerCount = playingDatas.Length;

            foreach (var pair in floatParameters)
            {
                animator.SetFloat(pair.Key, pair.Value);
            }
            foreach (var pair in boolParameters)
            {
                animator.SetBool(pair.Key, pair.Value);
            }
            foreach (var pair in intParameters)
            {
                animator.SetInteger(pair.Key, pair.Value);
            }
            foreach (string trigger in triggerParameters)
            {
                animator.SetTrigger(trigger);
            }

            for (int i = 0; i < layerCount; i++)
            {
                SerializableAnimatorPlayingData playingData = playingDatas[i];
                int currentNameHash = playingData.currentHash;
                float currentNormalizedTime = playingData.currentTime;

                animator.Play(currentNameHash, i, currentNormalizedTime);
            }

            for (int i = 0; i < layerCount; i++)
            {
                SerializableAnimatorPlayingData playingData = playingDatas[i];
                int nextFullPathHash = playingData.nextHash;
                if (nextFullPathHash != 0)
                {
                    float nextNormalizedTime = playingData.nextNormalizedTime;
                    float nextLength = playingData.nextLength;

                    var transitionDurationUnit = (DurationUnit)playingData.transitionDurationUnit;
                    float transitionDuration = playingData.transitionDuration;
                    float transitionNormalizedTime = playingData.transitionTime;
                    if (transitionDurationUnit == DurationUnit.Fixed)
                    {
                        animator.CrossFadeInFixedTime(nextFullPathHash, transitionDuration, i, nextLength, transitionNormalizedTime);
                    }
                    else
                    {
                        animator.CrossFade(nextFullPathHash, transitionDuration, i, nextNormalizedTime, transitionNormalizedTime);
                    }
                }
            }
            animator.Update(0);
        }
    }
    public class SerializableAnimatorPlayingData
    {
        public int currentHash;
        public float currentTime;
        public int nextHash;
        public float nextNormalizedTime;
        public float nextLength;
        public int transitionDurationUnit;
        public float transitionDuration;
        public float transitionTime;
    }
}
