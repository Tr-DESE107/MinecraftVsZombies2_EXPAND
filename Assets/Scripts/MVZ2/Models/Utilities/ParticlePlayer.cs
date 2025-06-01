using System;
using System.Linq;
using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(ParticleSystem))]
    public class ParticlePlayer : MonoBehaviour
    {
        public void SetSimulationSpeed(float speed)
        {
            var main = Particles.main;
            main.simulationSpeed = speed;
        }
        public SerializableParticleSystem ToSerializable()
        {
            return new SerializableParticleSystem(Particles);
        }
        public void LoadFromSerializable(SerializableParticleSystem serializable)
        {
            serializable.Load(Particles);
        }
        public float GetAmountMultiplier()
        {
            var multiplier = MainManager.Instance.OptionsManager.GetParticleAmount();
            return multiplier * (1 - minAmount) + minAmount;
        }
        private void Awake()
        {
            float multiplier = GetAmountMultiplier();

            var main = Particles.main;
            main.maxParticles = Mathf.RoundToInt(main.maxParticles * multiplier);

            var emission = Particles.emission;
            emission.rateOverTimeMultiplier *= multiplier;
            emission.rateOverDistanceMultiplier *= multiplier;
            for (int i = 0; i < emission.burstCount; i++)
            {
                var burst = emission.GetBurst(i);
                burst.count = MultiplyCurve(burst.count, multiplier);
                emission.SetBurst(i, burst);
            }
        }
        private void Update()
        {
            if (particlesToEmit > 0 && particles.main.simulationSpeed > 0)
            {
                Particles.Emit(particlesToEmit);
                particlesToEmit = 0;
            }
        }
        public void OverrideRateOverTime(float rate)
        {
            var emission = Particles.emission;
            emission.rateOverTimeMultiplier = rate * GetAmountMultiplier();
        }
        public void OverrideRateOverDistance(float rate)
        {
            var emission = Particles.emission;
            emission.rateOverDistanceMultiplier = rate * GetAmountMultiplier();
        }
        public void Emit(float count)
        {
            var intCount = (int)(count * GetAmountMultiplier());
            emitModular += count - intCount;
            if (emitModular > 1)
            {
                intCount += (int)emitModular;
                emitModular %= 1;
            }
            if (intCount <= 0)
                return;
            particlesToEmit += intCount;
        }
        public static ParticleSystem.MinMaxCurve MultiplyCurve(ParticleSystem.MinMaxCurve curve, float multiplier)
        {
            switch (curve.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    return new ParticleSystem.MinMaxCurve()
                    {
                        mode = curve.mode,
                        constant = curve.constant * multiplier,
                    };
                case ParticleSystemCurveMode.TwoConstants:
                    return new ParticleSystem.MinMaxCurve()
                    {
                        mode = curve.mode,
                        constantMax = curve.constantMax * multiplier,
                        constantMin = curve.constantMin * multiplier,
                    };
                case ParticleSystemCurveMode.Curve:
                    return new ParticleSystem.MinMaxCurve()
                    {
                        mode = curve.mode,
                        curve = curve.curve,
                        curveMultiplier = curve.curveMultiplier * multiplier
                    };
                case ParticleSystemCurveMode.TwoCurves:
                    return new ParticleSystem.MinMaxCurve()
                    {
                        mode = curve.mode,
                        curveMin = curve.curveMin,
                        curveMax = curve.curveMax,
                        curveMultiplier = curve.curveMultiplier * multiplier
                    };
            }
            return curve;
        }
        private void OnParticleCollision(GameObject other)
        {
            OnParticleCollisionEvent?.Invoke(this, other);
        }
        public ParticleSystem Particles
        {
            get
            {
                if (!particles)
                    particles = GetComponent<ParticleSystem>();
                return particles;
            }
        }
        public event Action<ParticlePlayer, GameObject> OnParticleCollisionEvent;
        private ParticleSystem particles;
        [SerializeField]
        private float minAmount = 0;
        private float emitModular = 0;
        private int particlesToEmit = 0;
    }
    public class SerializableParticleSystem
    {
        public int stateBeforePaused;
        public int state;
        public int seed;
        public float time;
        public SerializableParticle[] particles;
        public SerializableParticleSystem(ParticleSystem particleSystem)
        {
            seed = (int)particleSystem.randomSeed;
            time = particleSystem.time;
            state = (int)GetState(particleSystem);
            var parts = new ParticleSystem.Particle[particleSystem.particleCount];
            particleSystem.GetParticles(parts);
            particles = parts.Select(p => new SerializableParticle(p)).ToArray();
        }
        public void Load(ParticleSystem particleSystem)
        {
            particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
            particleSystem.randomSeed = (uint)seed;
            particleSystem.time = time;

            int count = particles.Length;
            var parts = new ParticleSystem.Particle[count];
            for (int i = 0; i < count; i++)
            {
                var particleData = particles[i];
                parts[i] = particleData.Deserialize();
            }
            particleSystem.SetParticles(parts);
            SetState((ParticleState)state, particleSystem);
        }
        private ParticleState GetState(ParticleSystem particleSystem)
        {
            if (particleSystem.isEmitting)
            {
                if (particleSystem.isPlaying)
                {
                    return ParticleState.EmittingAndPlaying;
                }
                return ParticleState.Emitting;
            }
            else if (particleSystem.isPlaying)
            {
                return ParticleState.Playing;
            }
            else if (particleSystem.isPaused)
            {
                return ParticleState.Pausing;
            }
            return ParticleState.Stopped;
        }
        private void SetState(ParticleState state, ParticleSystem particle)
        {
            switch (state)
            {
                case ParticleState.Playing:
                    particle.Play();
                    particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    break;
                case ParticleState.Pausing:
                    particle.Pause(false);
                    break;
                case ParticleState.EmittingAndPlaying:
                    particle.Play(false);
                    break;
                case ParticleState.Stopped:
                    if (particle.isPaused)
                    {
                        particle.Play();
                    }
                    particle.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    break;
            }
        }

        private enum ParticleState
        {
            Stopped,
            Pausing,
            Playing,
            Emitting,
            EmittingAndPlaying
        }
    }
    public class SerializableParticle
    {
        public Vector3 position;
        public Vector3 rotation3D;
        public Vector3 startSize3D;
        public Vector3 velocity;
        public Vector3 angularVelocity3D;
        public Vector3 axisOfRotation;

        public int randomSeed;
        public Color startColor;
        public float startLifetime;
        public float remainingLifetime;
        public SerializableParticle(ParticleSystem.Particle particle)
        {
            position = particle.position;
            rotation3D = particle.rotation3D;
            startSize3D = particle.startSize3D;
            velocity = particle.velocity;
            angularVelocity3D = particle.angularVelocity3D;
            axisOfRotation = particle.axisOfRotation;

            randomSeed = (int)particle.randomSeed;
            startColor = particle.startColor;
            startLifetime = particle.startLifetime;
            remainingLifetime = particle.remainingLifetime;
        }
        public ParticleSystem.Particle Deserialize()
        {
            return new ParticleSystem.Particle()
            {
                position = position,
                rotation3D = rotation3D,
                startSize3D = startSize3D,
                velocity = velocity,
                angularVelocity3D = angularVelocity3D,
                axisOfRotation = axisOfRotation,

                randomSeed = (uint)randomSeed,
                startColor = startColor,
                startLifetime = startLifetime,
                remainingLifetime = remainingLifetime,
            };
        }
    }
}
