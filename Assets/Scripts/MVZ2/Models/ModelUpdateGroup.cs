﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Models
{
    public class ModelUpdateGroup : MonoBehaviour
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
        public void UpdateElements()
        {
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

        public SerializableModelUpdateGroup ToSerializable()
        {
            var serializable = new SerializableModelUpdateGroup();
            serializable.animators = animators.Select(a => new SerializableAnimator(a)).ToArray();
            serializable.particles = particles.Select(e => e.ToSerializable()).ToArray();
            return serializable;
        }
        public void LoadFromSerializable(SerializableModelUpdateGroup serializable)
        {
            for (int i = 0; i < animators.Count; i++)
            {
                if (i >= serializable.animators.Length)
                    break;
                var animator = animators[i];
                var data = serializable.animators[i];
                data.Deserialize(animator);
            }
            for (int i = 0; i < particles.Count; i++)
            {
                if (i >= serializable.particles.Length)
                    break;
                var particle = particles[i];
                var data = serializable.particles[i];
                particle.LoadFromSerializable(data);
            }
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
        private static bool IsChildOfGroup(Transform child, ModelUpdateGroup group)
        {
            return child.GetComponentInParent<ModelUpdateGroup>() == group;
        }
        private static bool IsParticleChildOfGroup(Transform child, ModelUpdateGroup group)
        {
            return !child.parent.GetComponentInParent<ParticleSystem>() && IsChildOfGroup(child, group);
        }
        #endregion

        [SerializeField]
        private List<Animator> animators = new List<Animator>();
        [SerializeField]
        private List<ParticlePlayer> particles = new List<ParticlePlayer>();

    }
    public class SerializableModelUpdateGroup
    {
        public SerializableAnimator[] animators;
        public SerializableParticleSystem[] particles;
    }
}