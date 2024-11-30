using MVZ2.Managers;
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
            size *= MainManager.Instance.LevelManager.LawnToTransScale;
            float volume = size.x * size.z;
            float maxRadius = Mathf.Max(size.x, size.z);


            var explosionPs = explosionParticles.Particles;
            var smokePs = smokeParticles.Particles;

            var explosionShape = explosionPs.shape;
            var smokeMain = smokePs.main;

            explosionShape.scale = size;
            explosionPs.Emit(Mathf.CeilToInt(explosionParticleCount * volume * explosionParticles.GetAmountMultiplier()));

            smokeMain.startSpeedMultiplier = maxRadius * smokeSpeedMultiplier;
            smokePs.Emit(Mathf.CeilToInt(smokeParticleCount * volume * smokeParticles.GetAmountMultiplier()));
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
