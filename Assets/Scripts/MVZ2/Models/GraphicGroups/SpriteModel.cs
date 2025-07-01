using MVZ2Logic.HeldItems;
using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class SpriteModel : Model
    {
        public void SetLightVisible(bool visible)
        {
            lightController.gameObject.SetActive(visible);
        }
        public void SetLightColor(Color color)
        {
            lightController.SetColor(color);
        }
        public void SetLightRange(Vector2 range)
        {
            lightController.SetRange(range);
        }
        public void CancelSortAtRoot()
        {
            RendererGroup.CancelSortAtRoot();
        }
        public void SetColliderActive(bool active)
        {
            if (colliderObject)
            {
                colliderObject.SetActive(active);
            }
        }
        protected override SerializableModelData CreateSerializable()
        {
            var serializable = new SerializableSpriteModelData();
            serializable.light = lightController.ToSerializable();
            return serializable;
        }
        protected override void LoadSerializable(SerializableModelData serializable)
        {
            base.LoadSerializable(serializable);
            if (serializable is not SerializableSpriteModelData spriteModel)
                return;
            lightController.LoadFromSerializable(spriteModel.light);
        }
        public override ModelGraphicGroup GraphicGroup => RendererGroup;
        public Collider2D Collider => modelCollider;
        public HeldTargetFlag HeldTargetFlag => heldTargetFlag;
        public ModelSortedRendererGroup RendererGroup => rendererGroup;
        [Header("Sprite")]
        [SerializeField]
        private GameObject colliderObject;
        [SerializeField]
        private Collider2D modelCollider;
        [SerializeField]
        private HeldTargetFlag heldTargetFlag;
        [SerializeField]
        private ModelSortedRendererGroup rendererGroup;
        [SerializeField]
        private LightController lightController;
    }
    public class SerializableSpriteModelData : SerializableModelData
    {
        public SerializableLightController light;
    }
}