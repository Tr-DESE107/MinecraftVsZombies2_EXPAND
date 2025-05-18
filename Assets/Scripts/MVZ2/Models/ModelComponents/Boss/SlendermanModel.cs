using UnityEngine;

namespace MVZ2.Models
{
    public class SlendermanModel : ModelComponent
    {
        public override void Init()
        {
            base.Init();
            var rng = Model.GetRNG();
            foreach (var animator in tentacleAnimators)
            {
                animator.SetFloat("Speed", rng.Next(0.8f, 1.25f));
            }
        }
        [SerializeField]
        private Animator[] tentacleAnimators;
    }
}
