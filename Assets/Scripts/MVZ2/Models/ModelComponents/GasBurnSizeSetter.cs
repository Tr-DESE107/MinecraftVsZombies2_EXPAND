using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class GasBurnSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var size = Model.GetProperty<Vector3>("Size");
            size *= MainManager.Instance.LevelManager.LawnToTransScale;
            var volume = size.x * (size.y + size.z);

            var timeout = Model.GetProperty<int>("Timeout");
            var percentage = Mathf.Clamp01((maxTime - timeout) / (float)expandTime);

            var fireEmission = fires.emission;
            var fireShape = fires.shape;
            fireEmission.rateOverTime = volume * ratePerVolume * percentage;
            fireShape.scale = new Vector3(size.x, size.y + size.z, 1) * percentage;
        }
        [SerializeField]
        private ParticleSystem fires;
        [SerializeField]
        private int expandTime = 15;
        [SerializeField]
        private int maxTime = 45;
        [SerializeField]
        private float ratePerVolume = 100;
        public const string PROP_STOPPED = "Stopped";
    }
}
