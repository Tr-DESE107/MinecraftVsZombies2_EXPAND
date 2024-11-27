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


            var explosionShape = explosionParticles.shape;
            explosionShape.scale = size;
            explosionParticles.Emit(Mathf.CeilToInt(explosionParticleCount * volume));

            var smokeMain = smokeParticles.main;
            smokeMain.startSpeed = ParticlePlayer.MultiplyCurve(smokeMain.startSpeed, maxRadius * smokeSpeedMultiplier);
            smokeParticles.Emit(Mathf.CeilToInt(smokeParticleCount * volume));
        }
        [SerializeField]
        private ParticleSystem explosionParticles;
        [SerializeField]
        private ParticleSystem smokeParticles;
        [SerializeField]
        private float explosionParticleCount = 5;
        [SerializeField]
        private float smokeParticleCount = 5;
        [SerializeField]
        private float smokeSpeedMultiplier = 0.5f;
        public const string PROP_EMITTED = "Emitted";
    }
}
