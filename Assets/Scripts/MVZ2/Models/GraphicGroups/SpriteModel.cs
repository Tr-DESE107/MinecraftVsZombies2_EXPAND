using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class SpriteModel : Model
    {
        public void SetLight(bool visible, Vector2 range, Color color, Vector2 randomOffset)
        {
            lightController.gameObject.SetActive(visible);
            lightController.SetColor(color);
            lightController.SetRange(range, randomOffset);
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
        public ModelRendererGroup RendererGroup => rendererGroup;
        [Header("Sprite")]
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