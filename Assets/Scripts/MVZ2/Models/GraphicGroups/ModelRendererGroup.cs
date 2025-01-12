using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Models
{
    [RequireComponent(typeof(SortingGroup))]
    public class ModelRendererGroup : ModelGraphicGroup
    {
        #region 公有方法
        public override void SetSimulationSpeed(float speed)
        {
            base.SetSimulationSpeed(speed);
            foreach (var particle in particles)
            {
                particle.SetSimulationSpeed(speed);
            }
        }
        public override void UpdateElements()
        {
            renderers.Clear();
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
                renderers.Add(element);
            }
            transforms.Clear();
            foreach (var trans in GetComponentsInChildren<TransformElement>(true))
            {
                if (!IsChildOfGroup(trans.transform, this))
                    continue;
                transforms.Add(trans);
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
        public override void SetPropertyInt(string name, int value)
        {
            foreach (var element in GetAllElements())
            {
                element.SetInt(name, value);
            }
        }

        public override void SetPropertyFloat(string name, float alpha)
        {
            foreach (var element in GetAllElements())
            {
                element.SetFloat(name, alpha);
            }
        }

        public override void SetPropertyColor(string name, Color color)
        {
            foreach (var element in GetAllElements())
            {
                element.SetColor(name, color);
            }
        }
        #endregion

        #region 私有方法
        protected override SerializableModelGraphicGroup CreateSerializable()
        {
            var serializable = new SerializableModelRendererGroup();
            serializable.particles = particles.Select(e => e.ToSerializable()).ToArray();
            serializable.renderers = renderers.Select(e => e.ToSerializable()).ToArray();
            return serializable;
        }
        protected override void LoadSerializable(SerializableModelGraphicGroup serializable)
        {
            base.LoadSerializable(serializable);
            if (serializable is not SerializableModelRendererGroup spriteGroup)
                return;
            for (int i = 0; i < renderers.Count; i++)
            {
                if (i >= spriteGroup.renderers.Length)
                    break;
                var element = renderers[i];
                var data = spriteGroup.renderers[i];
                element.LoadFromSerializable(data);
            }
            for (int i = 0; i < particles.Count; i++)
            {
                if (i >= spriteGroup.particles.Length)
                    break;
                var particle = particles[i];
                var data = spriteGroup.particles[i];
                particle.LoadFromSerializable(data);
            }
        }
        private static bool IsParticleChildOfGroup(Transform child, ModelRendererGroup group)
        {
            return !child.parent.GetComponentInParent<ParticleSystem>() && IsChildOfGroup(child, group);
        }
        private RendererElement[] GetAllElements(bool includeExcluded = false)
        {
            if (includeExcluded)
                return renderers.ToArray();
            return renderers.Where(e => !e.ExcludedInGroup).ToArray();
        }
        #endregion

        #region 属性字段
        public override int SortingLayerID { get => sortingGroup.sortingLayerID; set => sortingGroup.sortingLayerID = value; }

        public override string SortingLayerName { get => sortingGroup.sortingLayerName; set => sortingGroup.sortingLayerName = value; }

        public override int SortingOrder { get => sortingGroup.sortingOrder; set => sortingGroup.sortingOrder = value; }

        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private List<RendererElement> renderers = new List<RendererElement>();
        [SerializeField]
        private List<ParticlePlayer> particles = new List<ParticlePlayer>();
        #endregion

    }
    public class SerializableModelRendererGroup : SerializableModelGraphicGroup
    {
        public SerializableParticleSystem[] particles;
        public SerializableRendererElement[] renderers;
    }
}