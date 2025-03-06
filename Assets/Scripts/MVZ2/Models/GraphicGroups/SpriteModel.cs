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
        public ModelRendererGroup RendererGroup => rendererGroup;
        [Header("Sprite")]
        [SerializeField]
        private Collider2D modelCollider;
        [SerializeField]
        private ModelRendererGroup rendererGroup;
        [SerializeField]
        private LightController lightController;
    }
    public class SerializableSpriteModelData : SerializableModelData
    {
        public SerializableLightController light;
    }
}