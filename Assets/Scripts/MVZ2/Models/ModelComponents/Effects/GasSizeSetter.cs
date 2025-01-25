using MVZ2.GameContent.Effects;
using UnityEngine;

namespace MVZ2.Models
{
    public class GasSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var size = Model.GetProperty<Vector3>("Size");
            size = Lawn2TransScale(size);
            var volume = size.x * (size.y + size.z);

            bool stopped = Model.GetProperty<bool>(PROP_STOPPED);
            if (gas)
            {
                var gasPS = gas.Particles;
                var gasEmission = gasPS.emission;
                var gasShape = gasPS.shape;
                gas.OverrideRateOverTime(volume * ratePerVolume);
                gasShape.scale = new Vector3(size.x, size.y + size.z, 1);
                if (stopped)
                {
                    if (gasPS.isEmitting)
                        gasPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    if (!gasPS.isEmitting)
                        gasPS.Play(true);
                }
            }

            if (gasLight)
            {
                var gasLightPS = gasLight.Particles;
                var gasLightEmission = gasLightPS.emission;
                var gasLightShape = gasLightPS.shape;
                gasLight.OverrideRateOverTime(volume * ratePerVolume);

                gasLightShape.scale = new Vector3(size.x, size.y + size.z, 1);

                if (stopped)
                {
                    if (gasLightPS.isEmitting)
                        gasLightPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    if (!gasLightPS.isEmitting)
                        gasLightPS.Play(true);
                }

            }

            if (smoke)
            {
                var smokePS = smoke.Particles;
                var smokeShape = smokePS.shape;
                smokeShape.scale = new Vector3(size.x, 1, 0.1f);
                if (stopped)
                {
                    if (smokePS.isEmitting)
                        smokePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                }
                else
                {
                    if (!smokePS.isEmitting)
                        smokePS.Play(true);
                }
            }
        }
        [SerializeField]
        private ParticlePlayer gasLight;
        [SerializeField]
        private ParticlePlayer gas;
        [SerializeField]
        private ParticlePlayer smoke;
        [SerializeField]
        private float ratePerVolume = 20;
        public const string PROP_STOPPED = "Stopped";
    }
}
