using UnityEngine;

namespace MVZ2.Models
{
    public class CartChargeBarModel : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var charge = Mathf.Clamp01(Model.GetProperty<float>("Charge"));
            chargeBarRoot.SetActive(charge > 0);
            circleSetter.fill = charge;
            chargeBarRenderer.color = barGradient.Evaluate(charge);
        }
        [SerializeField]
        private GameObject chargeBarRoot;
        [SerializeField]
        private SpriteRenderer chargeBarRenderer;
        [SerializeField]
        private CircleFillSpriteSetter circleSetter;
        [SerializeField]
        private Gradient barGradient;
    }
}
