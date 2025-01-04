using UnityEngine;

namespace MVZ2.Models
{
    public class KnockbackWaveModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            lastPosition = transform.position;
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            Vector3 transVelocity = transform.position - lastPosition;
            float angle = Vector2.SignedAngle(Vector2.right, transVelocity);
            SetDirection(angle);
            lastPosition = transform.position;
        }

        public void SetDirection(float angle)
        {
            particle.transform.eulerAngles = Vector3.forward * angle;
        }
        [SerializeField]
        private ParticleSystem particle;
        private Vector3 lastPosition;
    }
}
