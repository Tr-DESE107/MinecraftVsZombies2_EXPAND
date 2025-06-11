using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    public class HSVOffsetSetter : ShaderPropertySetter<Vector4>
    {
        public override Vector4 GetCurrentValue() => new Vector4(hue, saturation, value, 0);
        public override Vector4 GetDefaultValue() => Vector4.zero;
        public override void SetProperty(Vector4 value)
        {
            Element.SetVector("_HSVOffset", value);
        }
        [Range(-180, 180)]
        public float hue;
        [Range(-100, 100)]
        public float saturation;
        [Range(-100, 100)]
        public float value;
    }
}
