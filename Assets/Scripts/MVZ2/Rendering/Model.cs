using System.Collections.Generic;
using MVZ2.GameContent;
using MVZ2.Level;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Rendering
{
    [DisallowMultipleComponent]
    public sealed class Model : MonoBehaviour
    {
        #region 公有方法
        public void UpdateFixed()
        {
            triggeringEvents.Clear();
        }
        public void UpdateFrame(float deltaTime)
        {
            Animator.enabled = false;
            Animator.Update(deltaTime);

            var lightVisible = GetProperty<bool>(BuiltinModelProps.LIGHT_VISIBLE);
            var lightScale = GetProperty<Vector2>(BuiltinModelProps.LIGHT_RANGE);
            var lightColor = GetProperty<Color>(BuiltinModelProps.LIGHT_COLOR);
            rendererGroup.SetLight(lightVisible, lightScale, lightColor);
        }
        public void SetSimulationSpeed(float simulationSpeed)
        {
            RendererGroup.SetSimulationSpeed(simulationSpeed);
        }
        public void TriggerAnimator(string name)
        {
            Animator.SetTrigger(name);
        }
        public void SetAnimatorBool(string name, bool value)
        {
            Animator.SetBool(name, value);
        }
        public void SetAnimatorInt(string name, int value)
        {
            Animator.SetInteger(name, value);
        }
        public void SetAnimatorFloat(string name, float value)
        {
            Animator.SetFloat(name, value);
        }
        public bool IsEventTriggered(string name)
        {
            return triggeringEvents.Contains(name);
        }

        public bool WasEventTriggered(string name)
        {
            return triggeredEvents.Contains(name);
        }

        public SerializableModelData ToSerializable()
        {
            return new SerializableModelData()
            {
                animator = new SerializableAnimator(animator),
                armorModel = armorModel ? armorModel.ToSerializable() : null,
                rendererGroup = rendererGroup.ToSerializable(),
                triggeredEvents = triggeredEvents.ToArray(),
                triggeringEvents = triggeringEvents.ToArray()
            };
        }
        public void LoadFromSerializable(SerializableModelData serializable)
        {
            serializable.animator.Deserialize(animator);
            if (armorModel && serializable.armorModel != null)
            {
                armorModel.LoadFromSerializable(serializable.armorModel);
            }
            rendererGroup.LoadFromSerializable(serializable.rendererGroup);
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
        }
        #endregion

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
        public Animator Animator => animator;
        public Transform RootTransform => rootTransform;
        public Transform CenterTransform => centerTransform;
        public Model ArmorModel => armorModel;
        public float AnimationSpeed { get; set; }
        [SerializeField]
        private MultipleRendererGroup rendererGroup;
        [SerializeField]
        private Animator animator;
        [SerializeField]
        private Transform rootTransform;
        [SerializeField]
        private Transform centerTransform;
        [SerializeField]
        private Transform armorTransform;
        [SerializeField]
        private Model armorModel;
        private List<string> triggeringEvents = new List<string>();
        private List<string> triggeredEvents = new List<string>();
        private PropertyDictionary propertyDict = new PropertyDictionary();
        #endregion
    }
    public class SerializableModelData
    {
        public SerializableMultipleRendererGroup rendererGroup;
        public SerializableAnimator animator;
        public SerializableModelData armorModel;
        public string[] triggeringEvents;
        public string[] triggeredEvents;
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
            animator.Update(0);

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