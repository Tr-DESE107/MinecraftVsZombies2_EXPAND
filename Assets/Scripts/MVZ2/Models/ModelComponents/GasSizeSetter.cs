using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class GasSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var size = Model.GetProperty<Vector3>("Size");
            size *= MainManager.Instance.LevelManager.LawnToTransScale;
            var volume = size.x * (size.y + size.z);

            var gasLightEmission = gasLight.emission;
            var gasEmission = gas.emission;
            gasEmission.rateOverTime = volume * ratePerVolume;
            gasLightEmission.rateOverTime = volume * ratePerVolume;

            var gasLightShape = gasLight.shape;
            var gasShape = gas.shape;
            gasShape.scale = new Vector3(size.x, size.y + size.z, 1);
            gasLightShape.scale = new Vector3(size.x, size.y + size.z, 1);

            var smokeShape = smoke.shape;
            smokeShape.scale = new Vector3(size.x, 1, 0.1f);

            if (Model.GetProperty<bool>(PROP_STOPPED))
            {
                if (gas.isEmitting)
                    gas.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (gasLight.isEmitting)
                    gasLight.Stop(true, ParticleSystemStopBehavior.StopEmitting);
                if (smoke.isEmitting)
                    smoke.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            else
            {
                if (!gas.isEmitting)
                    gas.Play(true);
                if (!gasLight.isEmitting)
                    gasLight.Play(true);
                if (!smoke.isEmitting)
                    smoke.Play(true);
            }
        }
        [SerializeField]
        private ParticleSystem gasLight;
        [SerializeField]
        private ParticleSystem gas;
        [SerializeField]
        private ParticleSystem smoke;
        [SerializeField]
        private float ratePerVolume = 20;
        public const string PROP_STOPPED = "Stopped";
    }
}
