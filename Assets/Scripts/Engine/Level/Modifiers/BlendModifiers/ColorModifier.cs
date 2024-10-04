using PVZEngine.Level;
using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class ColorModifier : PropertyModifier<Color>
    {
        public ColorModifier(string propertyName, Color valueConst) : this(propertyName, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha, valueConst)
        {
        }
        public ColorModifier(string propertyName, string buffPropertyName) : this(propertyName, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha, buffPropertyName)
        {
        }
        public ColorModifier(string propertyName, BlendOperator src, BlendOperator dest, Color valueConst) : base(propertyName, valueConst)
        {
            SrcOperator = src;
            DstOperator = dest;
        }

        public ColorModifier(string propertyName, BlendOperator src, BlendOperator dest, string buffPropertyName) : base(propertyName, buffPropertyName)
        {
            SrcOperator = src;
            DstOperator = dest;
        }
        public BlendOperator SrcOperator { get; private set; }
        public BlendOperator DstOperator { get; private set; }
    }
}
