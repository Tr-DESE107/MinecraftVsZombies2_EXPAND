using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class SmokeSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            if (Model.GetProperty<bool>(PROP_EMITTED))
                return;
            Model.SetProperty(PROP_EMITTED, true);
            var size = Model.GetProperty<Vector3>("Size");
            size *= MainManager.Instance.LevelManager.LawnToTransScale;
            var shape = particles.shape;
            int volume = Mathf.CeilToInt(100 * size.x * size.y * size.z);
            var scale = size;
            scale.y *= 0.333333f;
            shape.scale = scale;
            shape.position = Vector2.up * scale.y * 0.5f;
            particles.Emit(volume);
        }
        [SerializeField]
        private ParticleSystem particles;
        public const string PROP_EMITTED = "Emitted";
    }
}
