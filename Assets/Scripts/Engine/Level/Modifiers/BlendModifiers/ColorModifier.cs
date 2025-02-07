using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class ColorModifier : PropertyModifier<Color>
    {
        public ColorModifier(PropertyKey propertyName, Color valueConst) : this(propertyName, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha, valueConst)
        {
        }
        public ColorModifier(PropertyKey propertyName, PropertyKey buffPropertyName) : this(propertyName, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha, buffPropertyName)
        {
        }
        public ColorModifier(PropertyKey propertyName, BlendOperator src, BlendOperator dest, Color valueConst) : base(propertyName, valueConst)
        {
            SrcOperator = src;
            DstOperator = dest;
        }

        public ColorModifier(PropertyKey propertyName, BlendOperator src, BlendOperator dest, PropertyKey buffPropertyName) : base(propertyName, buffPropertyName)
        {
            SrcOperator = src;
            DstOperator = dest;
        }
        public BlendOperator SrcOperator { get; private set; }
        public BlendOperator DstOperator { get; private set; }
        public static ColorModifier Multiply(PropertyKey propertyName, Color valueConst)
        {
            return new ColorModifier(propertyName, BlendOperator.DstColor, BlendOperator.Zero, valueConst);
        }
        public static ColorModifier Multiply(PropertyKey propertyName, PropertyKey buffPropertyName)
        {
            return new ColorModifier(propertyName, BlendOperator.DstColor, BlendOperator.Zero, buffPropertyName);
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.colorCalculator;
        }
    }
}
