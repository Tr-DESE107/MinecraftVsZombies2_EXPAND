using System;
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
        public override void AddElement(GraphicElement element)
        {
            if (element is not RendererElement rendererElement)
                throw new ArgumentException("Wrong model group element type.", nameof(element));
            renderers.Add(rendererElement);
        }
        public void CancelSortAtRoot()
        {
            foreach (var element in subSortingGroups)
            {
                if (element.ExcludedInGroup)
                    continue;
                var group = element.Group;
                group.sortAtRoot = false;
            }
        }
        private void ReplaceComponents<T>(List<T> list, IEnumerable<T> targets)
        {
            list.RemoveAll(e => !targets.Contains(e));
            list.AddRange(targets.Except(list));
        }
        public override void UpdateElements()
        {
            var groups = GetComponentsInChildren<SortingGroup>(true)
                .Where(g => IsChildOfGroup(g.transform, this) && g.gameObject != gameObject)
                .Select(r => {
                    var element = r.GetComponent<SortingGroupElement>();
                    if (!element)
                    {
                        element = r.gameObject.AddComponent<SortingGroupElement>();
                    }
                    return element;
                });
            ReplaceComponents(subSortingGroups, groups);

            var renderer = GetComponentsInChildren<Renderer>(true)
                .Where(g => IsChildOfGroup(g.transform, this) && g is not SpriteMask && g.gameObject.layer != Layers.LIGHT)
                .Select(r => {
                    var element = r.GetComponent<RendererElement>();
                    if (!element)
                    {
                        element = r.gameObject.AddComponent<RendererElement>();
                    }
                    return element;
                });
            ReplaceComponents(renderers, renderer);

            var trans = GetComponentsInChildren<TransformElement>(true)
                .Where(g => IsChildOfGroup(g.transform, this));
            ReplaceComponents(transforms, trans);

            var parts = GetComponentsInChildren<ParticleSystem>(true)
                .Where(p => IsParticleChildOfGroup(p.transform, this))
                .Select(r => {
                     var element = r.GetComponent<ParticlePlayer>();
                     if (!element)
                     {
                         element = r.gameObject.AddComponent<ParticlePlayer>();
                     }
                     return element;
                 });
            ReplaceComponents(particles, parts);

            var anims = GetComponentsInChildren<Animator>(true)
                .Where(g => IsChildOfGroup(g.transform, this));
            ReplaceComponents(animators, anims);
        }
        public override void SetShaderInt(string name, int value)
        {
            foreach (var element in renderers)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                element.SetInt(name, value);
            }
        }

        public override void SetShaderFloat(string name, float alpha)
        {
            foreach (var element in renderers)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                element.SetFloat(name, alpha);
            }
        }

        public override void SetShaderColor(string name, Color color)
        {
            foreach (var element in renderers)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                element.SetColor(name, color);
            }
        }
        public override void SetShaderVector(string name, Vector4 vector)
        {
            foreach (var element in renderers)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                element.SetVector(name, vector);
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
        #endregion

        #region 属性字段
        public override int SortingLayerID 
        { 
            get => sortingGroup.sortingLayerID; 
            set
            {
                sortingGroup.sortingLayerID = value;
                foreach (var element in subSortingGroups)
                {
                    if (element.ExcludedInGroup)
                        continue;
                    var group = element.Group;
                    if (!group.sortAtRoot)
                        continue;
                    group.sortingLayerID = value;
                }
            }
        }
        public override string SortingLayerName 
        { 
            get => sortingGroup.sortingLayerName;
            set
            {
                sortingGroup.sortingLayerName = value;
                foreach (var element in subSortingGroups)
                {
                    if (element.ExcludedInGroup)
                        continue;
                    var group = element.Group;
                    if (!group.sortAtRoot)
                        continue;
                    group.sortingLayerName = value;
                }
            }
        }
        public override int SortingOrder 
        { 
            get => sortingGroup.sortingOrder;
            set
            {
                sortingGroup.sortingOrder = value;
                foreach (var element in subSortingGroups)
                {
                    if (element.ExcludedInGroup)
                        continue;
                    var group = element.Group;
                    if (!group.sortAtRoot)
                        continue;
                    group.sortingOrder = value;
                }
            }
        }

        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private List<SortingGroupElement> subSortingGroups = new List<SortingGroupElement>();
        [SerializeField]
        private List<RendererElement> renderers = new List<RendererElement>();
        [SerializeField]
        private List<ParticlePlayer> particles = new List<ParticlePlayer>();
        #endregion

    }
    public class SerializableModelRendererGroup : SerializableModelGraphicGroup
    {
        public SerializableParticleSystem[] particles;
        public SerializableGraphicElement[] renderers;
    }
}