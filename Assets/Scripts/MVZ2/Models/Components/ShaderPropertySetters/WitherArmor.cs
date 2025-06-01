using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class WitherArmor : ShaderPropertySetter<Vector4>
    {
        public override void SetProperty(Vector4 value)
        {
            Element.SetVector("_MainTex_ST", value);
        }
        public override Vector4 GetDefaultValue() => new Vector4(1, 1, 0, 0);
        public override Vector4 GetCurrentValue() 
        {
            var offset = Model.GetProperty<Vector2>("Offset");
            return new Vector4(1, 1, offset.x, offset.y);
        }
    }
}
