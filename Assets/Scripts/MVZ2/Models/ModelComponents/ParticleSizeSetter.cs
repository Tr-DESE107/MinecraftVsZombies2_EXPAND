using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    public class ParticleSizeSetter : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var size = Model.GetProperty<Vector3>("Size");
            size *= MainManager.Instance.LevelManager.LawnToTransScale;

            var shape = particles.shape;
            shape.scale = size;
            shape.position = Vector2.up * size.y * 0.5f;
        }
        [SerializeField]
        private ParticleSystem particles;
    }
}
