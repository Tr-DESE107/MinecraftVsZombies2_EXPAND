using UnityEngine;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    public sealed class UIModel : Model
    {
        public override void Init(Camera camera, int seed = 0)
        {
            base.Init(camera, seed);
            imageGroup.SetCamera(camera);
        }
        protected override SerializableModelData CreateSerializable()
        {
            var serializable = new SerializableUIModelData();
            return serializable;
        }
        protected override void LoadSerializable(SerializableModelData serializable)
        {
            base.LoadSerializable(serializable);
        }
        public override ModelGraphicGroup GraphicGroup => ImageGroup;
        public ModelImageGroup ImageGroup => imageGroup;
        [Header("Image")]
        [SerializeField]
        private ModelImageGroup imageGroup;
    }
    public class SerializableUIModelData : SerializableModelData
    {
    }
}