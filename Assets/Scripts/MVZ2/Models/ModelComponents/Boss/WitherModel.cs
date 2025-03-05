using MVZ2.Audios;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Models
{
    public class WitherModel : ModelComponent
    {
        public override void UpdateLogic()
        {
            base.UpdateLogic();
            var offset = Model.GetProperty<Vector2>("Offset");
            offset += armorOffsetSpeed;
            offset.x %= 1;
            offset.y %= 1;
            Model.SetProperty("Offset", offset);
        }
        public override void UpdateFrame(float deltaTime)
        {
            base.UpdateFrame(deltaTime);
            var offset = Model.GetProperty<Vector2>("Offset");
            var block = PropertyBlock;
            foreach (var renderer in armorRenderers)
            {
                renderer.GetPropertyBlock(block);
                block.SetVector("_BaseMap_ST", new Vector4(1, 1, offset.x, offset.y));
                renderer.SetPropertyBlock(block);
            }
        }
        private MaterialPropertyBlock PropertyBlock
        {
            get
            {
                if (propertyBlock == null)
                {
                    propertyBlock = new MaterialPropertyBlock();
                }
                return propertyBlock;
            }
        }
        private MaterialPropertyBlock propertyBlock;
        [SerializeField]
        private Renderer[] armorRenderers;
        [SerializeField]
        private Vector2 armorOffsetSpeed = new Vector2(0.03f, 0.01f);
    }
}
