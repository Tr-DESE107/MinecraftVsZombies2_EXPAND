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
            particles.Emit(modified);
        }
        [SerializeField]
        private ParticlePlayer particles;
        [SerializeField]
        private float emitSpeedMultiplier = 0.1f;
    }
}
