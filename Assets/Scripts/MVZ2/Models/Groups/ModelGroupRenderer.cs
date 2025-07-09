﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Models
{
    public abstract class ModelGroupRenderer : ModelGroup
    {
        #region 粒子
        public override void SetSimulationSpeed(float speed)
        {
            base.SetSimulationSpeed(speed);
            foreach (var particle in particles)
            {
                particle.SetSimulationSpeed(speed);
            }
        }
        #endregion

        #region 元素管理
        public override void AddElement(GraphicElement element)
        {
            if (element is not RendererElement rendererElement)
                throw new ArgumentException("Wrong model group element type.", nameof(element));
            renderers.Add(rendererElement);
        }
        public override void UpdateElements()
        {
            base.UpdateElements();
            var newRenderers = GetComponentsInChildren<Renderer>(true)
                .Where(g => g.IsDirectChild<ModelGroup>(this) && g is not SpriteMask && g.gameObject.layer != Layers.LIGHT)
                .Select(r =>
                {
                    var element = r.GetComponent<RendererElement>();
                    if (!element)
                    {
                        element = r.gameObject.AddComponent<RendererElement>();
                    }
                    return element;
                });
            renderers.ReplaceList(newRenderers);

            var newParticles = GetComponentsInChildren<ParticleSystem>(true)
                .Where(p => p.IsDirectChild<ModelGroup>(this) && !p.transform.parent.GetComponentInParent<ParticleSystem>())
                .Select(r =>
                {
                    var element = r.GetComponent<ParticlePlayer>();
                    if (!element)
                    {
                        element = r.gameObject.AddComponent<ParticlePlayer>();
                    }
                    return element;
                });
            particles.ReplaceList(newParticles);
        }
        #endregion

        #region 序列化
        protected void SaveToSerializableRenderer(SerializableModelGroupRenderer serializable)
        {
            SaveToSerializableGroup(serializable);
            serializable.particles = particles.Select(e => e.ToSerializable()).ToArray();
            serializable.renderers = renderers.Select(e => e.ToSerializable()).ToArray();
        }
        protected override void LoadFromSerializable(SerializableModelGroup serializable)
        {
            base.LoadFromSerializable(serializable);
            if (serializable is not SerializableModelGroupRenderer rendererUnit)
                return;
            if (renderers != null)
            {
                for (int i = 0; i < renderers.Count; i++)
                {
                    if (i >= rendererUnit.renderers.Length)
                        break;
                    var element = renderers[i];
                    var data = rendererUnit.renderers[i];
                    element.LoadFromSerializable(data);
                }
            }
            if (particles != null)
            {
                for (int i = 0; i < particles.Count; i++)
                {
                    if (i >= rendererUnit.particles.Length)
                        break;
                    var particle = particles[i];
                    var data = rendererUnit.particles[i];
                    particle.LoadFromSerializable(data);
                }
            }
        }
        #endregion

        #region 着色器
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

        #region 属性字段
        [SerializeField]
        protected List<RendererElement> renderers = new List<RendererElement>();
        [SerializeField]
        protected List<ParticlePlayer> particles = new List<ParticlePlayer>();
        #endregion
    }
    public abstract class SerializableModelGroupRenderer : SerializableModelGroup
    {
        public SerializableParticleSystem[] particles;
        public SerializableGraphicElement[] renderers;
    }
}