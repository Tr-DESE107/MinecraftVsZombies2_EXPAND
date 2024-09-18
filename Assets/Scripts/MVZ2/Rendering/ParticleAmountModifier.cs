using UnityEngine;

namespace MVZ2.Rendering
{
    public class ParticleAmountModifier : MonoBehaviour
    {
        private void Awake()
        {
            float multiplier = MainManager.Instance.OptionsManager.GetParticleAmount();

            var main = particles.main;
            main.maxParticles = Mathf.RoundToInt(main.maxParticles * multiplier);

            var emission = particles.emission;
            emission.rateOverTimeMultiplier *= multiplier;
            emission.rateOverDistanceMultiplier *= multiplier;
            for (int i = 0; i < emission.burstCount; i++)
            {
                var burst = emission.GetBurst(i);
                burst.count = MultiplyCurve(burst.count, multiplier);
                emission.SetBurst(i, burst);
            }
        }
        private ParticleSystem.MinMaxCurve MultiplyCurve(ParticleSystem.MinMaxCurve curve, float multiplier)
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
        [SerializeField]
        private ParticleSystem particles;
    }
}
