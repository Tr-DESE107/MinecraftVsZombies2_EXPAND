using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Rendering
{
    [RequireComponent(typeof(SortingGroup))]
    public class MultipleRendererGroup : MonoBehaviour
    {
        #region 公有方法
        public void UpdateFrame(float deltaTime)
        {
            foreach (var animator in animators)
            {
                animator.enabled = false;
                animator.Update(deltaTime);
            }
        }
        public void SetSimulationSpeed(float speed)
        {
            foreach (var particle in particles)
            {
                particle.SetSimulationSpeed(speed);
            }
        }
        public void SetLight(bool visible, Vector2 range, Color color)
        {
            lightController.gameObject.SetActive(visible);
            lightController.SetColor(color);
            lightController.SetRange(range);
        }
        public void UpdateRendererElements()
        {
            elements.Clear();
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                if (!IsChildOfGroup(renderer.transform, this))
                    continue;
                if (renderer is SpriteMask)
                    continue;
                if (renderer.gameObject.layer == Layers.LIGHT)
                    continue;
                var element = renderer.GetComponent<RendererElement>();
                if (!element)
                {
                    element = renderer.gameObject.AddComponent<RendererElement>();
                }
                elements.Add(element);
            }

            particles.Clear();
            foreach (var ps in GetComponentsInChildren<ParticleSystem>(true))
            {
                if (!IsParticleChildOfGroup(ps.transform, this))
                    continue;
                var player = ps.GetComponent<ParticlePlayer>();
                if (!player)
                {
                    player = ps.gameObject.AddComponent<ParticlePlayer>();
                }
                particles.Add(player);
            }

            animators.Clear();
            foreach (var animator in GetComponentsInChildren<Animator>(true))
            {
                if (!IsChildOfGroup(animator.transform, this))
                    continue;
                animators.Add(animator);
            }
        }
        public void SetGroundPosition(Vector3 position)
        {
            foreach (var element in elements)
            {
                if (!element.LockToGround)
                    continue;
                var trans = element.transform;
                trans.position = position;
            }
        }
        public void SetPropertyInt(string name, int value)
        {
            foreach (var element in GetAllElements())
            {
                element.SetInt(name, value);
            }
        }

        public void SetPropertyFloat(string name, float alpha)
        {
            foreach (var element in GetAllElements())
            {
                element.SetFloat(name, alpha);
            }
        }

        public void SetPropertyColor(string name, Color color)
        {
            foreach (var element in GetAllElements())
            {
                element.SetColor(name, color);
            }
        }
        public void TriggerAnimator(string name)
        {
            foreach (var animator in animators)
            {
                animator.SetTrigger(name);
            }
        }
        public void SetAnimatorBool(string name, bool value)
        {
            foreach (var animator in animators)
            {
                animator.SetBool(name, value);
            }
        }
        public void SetAnimatorInt(string name, int value)
        {
            foreach (var animator in animators)
            {
                animator.SetInteger(name, value);
            }
        }
        public void SetAnimatorFloat(string name, float value)
        {
            foreach (var animator in animators)
            {
                animator.SetFloat(name, value);
            }
        }
        public void SetTint(Color color)
        {
            SetPropertyColor("_Color", color);
        }
        public void SetColorOffset(Color color)
        {
            SetPropertyColor("_ColorOffset", color);
        }

        public SerializableMultipleRendererGroup ToSerializable()
        {
            var serializable = new SerializableMultipleRendererGroup();
            serializable.animators = animators.Select(a => new SerializableAnimator(a)).ToArray();
            serializable.sortingLayerID = SortingLayerID;
            serializable.sortingOrder = SortingOrder;
            serializable.elements = elements.Select(e => e.ToSerializable()).ToArray();
            serializable.particles = particles.Select(e => e.ToSerializable()).ToArray();
            serializable.light = lightController.ToSerializable();
            return serializable;
        }
        public void LoadFromSerializable(SerializableMultipleRendererGroup serializable)
        {
            SortingLayerID = serializable.sortingLayerID;
            SortingOrder = serializable.sortingOrder;
            for (int i = 0; i < animators.Count; i++)
            {
                if (i >= serializable.animators.Length)
                    break;
                var animator = animators[i];
                var data = serializable.animators[i];
                data.Deserialize(animator);
            }
            for (int i = 0; i < elements.Count; i++)
            {
                if (i >= serializable.elements.Length)
                    break;
                var element = elements[i];
                var data = serializable.elements[i];
                element.LoadFromSerializable(data);
            }
            for (int i = 0; i < particles.Count; i++)
            {
                if (i >= serializable.particles.Length)
                    break;
                var particle = particles[i];
                var data = serializable.particles[i];
                particle.LoadFromSerializable(data);
            }
            lightController.LoadFromSerializable(serializable.light);
        }
        #endregion

        #region 私有方法
        private void Awake()
        {
            foreach (var animator in animators)
            {
                animator.logWarnings = false;
            }
        }
        private static bool IsChildOfGroup(Transform child, MultipleRendererGroup group)
        {
            return child.GetComponentInParent<MultipleRendererGroup>() == group;
        }
        private static bool IsParticleChildOfGroup(Transform child, MultipleRendererGroup group)
        {
            return !child.parent.GetComponentInParent<ParticleSystem>() && IsChildOfGroup(child, group);
        }
        private RendererElement[] GetAllElements(bool includeExcluded = false)
        {
            if (includeExcluded)
                return elements.ToArray();
            return elements.Where(e => !e.ExcludedInGroup).ToArray();
        }
        #endregion
        public int SortingLayerID { get => sortingGroup.sortingLayerID; set => sortingGroup.sortingLayerID = value; }

        public string SortingLayerName { get => sortingGroup.sortingLayerName; set => sortingGroup.sortingLayerName = value; }

        public int SortingOrder { get => sortingGroup.sortingOrder; set => sortingGroup.sortingOrder = value; }

        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private LightController lightController;
        [SerializeField]
        private List<Animator> animators;
        [SerializeField]
        private List<RendererElement> elements = new List<RendererElement>();
        [SerializeField]
        private List<ParticlePlayer> particles = new List<ParticlePlayer>();

    }
    public class SerializableMultipleRendererGroup
    {
        public SerializableAnimator[] animators;
        public SerializableRendererElement[] elements;
        public SerializableParticleSystem[] particles;
        public SerializableLightController light;
        public int sortingLayerID;
        public int sortingOrder;
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