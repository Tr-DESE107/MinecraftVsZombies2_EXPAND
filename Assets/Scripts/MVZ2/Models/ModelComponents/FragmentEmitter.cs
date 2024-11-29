using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class FragmentEmitter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var modified = emitSpeedMultiplier * Model.GetProperty<float>("EmitSpeed");
            modified *= MainManager.Instance.OptionsManager.GetParticleAmount();
            var count = (int)modified;
            modular += modified - count;
            if (modular > 1)
            {
                count += (int)modular;
                modular %= 1;
            }
            particles.Emit(count);
        }
        [SerializeField]
        private ParticleSystem particles;
        [SerializeField]
        private float emitSpeedMultiplier = 0.1f;
        private float modular = 0;
    }
}
