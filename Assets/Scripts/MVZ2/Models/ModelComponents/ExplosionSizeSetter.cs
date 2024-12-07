using UnityEngine;

namespace MVZ2.Models
{
    public class ExplosionSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (Model.GetProperty<bool>(PROP_EMITTED))
                return;
            Model.SetProperty(PROP_EMITTED, true);


            var size = Model.GetProperty<Vector3>("Size");
            size = Lawn2TransScale(size);
            float volume = size.x * size.z;
            float maxRadius = Mathf.Max(size.x, size.z);


            var explosionPs = explosionParticles.Particles;
            var smokePs = smokeParticles.Particles;

            var explosionShape = explosionPs.shape;
            var smokeMain = smokePs.main;

            explosionShape.scale = size;
            explosionParticles.Emit(explosionParticleCount * volume);

            smokeMain.startSpeedMultiplier = maxRadius * smokeSpeedMultiplier;
            smokeParticles.Emit(smokeParticleCount * volume);
        }
        [SerializeField]
        private ParticlePlayer explosionParticles;
        [SerializeField]
        private ParticlePlayer smokeParticles;
        [SerializeField]
        private float explosionParticleCount = 5;
        [SerializeField]
        private float smokeParticleCount = 5;
        [SerializeField]
        private float smokeSpeedMultiplier = 0.5f;
        public const string PROP_EMITTED = "Emitted";
    }
}
