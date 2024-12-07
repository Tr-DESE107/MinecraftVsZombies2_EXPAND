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

            var gasPS = gas.Particles;
            var gasLightPS = gasLight.Particles;
            var smokePS = smoke.Particles;

            var gasLightEmission = gasLightPS.emission;
            var gasLightShape = gasLightPS.shape;
            var gasEmission = gasPS.emission;
            var gasShape = gasPS.shape;
            var smokeShape = smokePS.shape;
            gas.OverrideRateOverTime(volume * ratePerVolume);
            gasLight.OverrideRateOverTime(volume * ratePerVolume);

            gasShape.scale = new Vector3(size.x, size.y + size.z, 1);
            gasLightShape.scale = new Vector3(size.x, size.y + size.z, 1);

            smokeShape.scale = new Vector3(size.x, 1, 0.1f);

            if (Model.GetProperty<bool>(PROP_STOPPED))
            {
                if (gasPS.isEmitting)
                    gasPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (gasLightPS.isEmitting)
                    gasLightPS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (smokePS.isEmitting)
                    smokePS.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            else
            {
                if (!gasPS.isEmitting)
                    gasPS.Play(true);
                if (!gasLightPS.isEmitting)
                    gasLightPS.Play(true);
                if (!smokePS.isEmitting)
                    smokePS.Play(true);
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
