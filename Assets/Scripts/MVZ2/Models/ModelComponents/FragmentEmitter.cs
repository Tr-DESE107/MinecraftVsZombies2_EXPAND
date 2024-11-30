using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class FragmentEmitter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var modified = Model.GetProperty<float>("EmitSpeed");
            modified *= emitSpeedMultiplier;
            modified *= particles.GetAmountMultiplier();
            var count = (int)modified;
            modular += modified - count;
            if (modular > 1)
            {
                count += (int)modular;
                modular %= 1;
            }
            particles.Particles.Emit(count);
        }
        [SerializeField]
        private ParticlePlayer particles;
        [SerializeField]
        private float emitSpeedMultiplier = 0.1f;
        private float modular = 0;
    }
}
