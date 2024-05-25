using UnityEngine;

namespace MVZ2.Rendering
{
    public class ParticleAnimator : MonoBehaviour
    {
        public void Emit()
        {
            var count = (int)emitCount;
            modular += emitCount - count;
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
        private float emitCount = 1;
        private float modular = 0;
    }
}
