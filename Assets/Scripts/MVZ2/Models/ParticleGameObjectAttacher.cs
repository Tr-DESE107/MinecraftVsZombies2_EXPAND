using System.Collections.Generic;
using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticleGameObjectAttacher : MonoBehaviour
    {
        private void OnDisable()
        {
            Clear();
        }
        void LateUpdate()
        {
            if (!Main)
                return;
            if (system == null)
            {
                system = GetComponent<ParticleSystem>();
                particles = new ParticleSystem.Particle[system.main.maxParticles];
            }
            int count = system.GetParticles(particles);

            while (instances.Count < count)
            {
                Pop();
            }

            bool worldSpace = (system.main.simulationSpace == ParticleSystemSimulationSpace.World);
            for (int i = 0; i < instances.Count; i++)
            {
                var instance = instances[i];
                if (i < count)
                {
                    var particle = particles[i];
                    if (worldSpace)
                    {
                        instance.transform.position = particle.position;
                    }
                    else
                    {
                        instance.transform.position = transform.position + particle.position;
                    }
                    instance.SetColor(color);
                    if (scaleMethod == ScaleMethod.Lifetime)
                    {
                        instance.transform.localScale = transform.lossyScale * (particle.remainingLifetime / particle.startLifetime * scale);
                    }
                    else
                    {
                        var current = particle.GetCurrentSize3D(system);
                        var start = particle.startSize3D;
                        var lossy = transform.lossyScale;
                        var x = current.x / start.x * scale * lossy.x;
                        var y = current.y / start.y * scale * lossy.y;
                        var z = current.z / start.z * scale * lossy.y;
                        instance.transform.localScale = new Vector3(x, y, z);
                    }
                }
                else
                {
                    Push(instance);
                }
            }
        }
        private LightController Pop()
        {
            LightController instance = ParticleManager.PopLight();
            instances.Add(instance);
            return instance;
        }
        private void Push(LightController instance)
        {
            ParticleManager.PushLight(instance);
            instances.Remove(instance);
        }
        private void Clear()
        {
            foreach (var instance in instances)
            {
                ParticleManager.PushLight(instance);
            }
            instances.Clear();
        }

        public MainManager Main => MainManager.Instance;
        public ParticleManager ParticleManager => Main.ParticleManager;
        [SerializeField]
        public ScaleMethod scaleMethod;
        [SerializeField]
        public float scale = 1;
        [SerializeField]
        public Color color = Color.white;

        private ParticleSystem system;
        private List<LightController> instances = new List<LightController>();
        private ParticleSystem.Particle[] particles;
    }

    public enum ScaleMethod
    {
        Lifetime,
        Size
    }
}