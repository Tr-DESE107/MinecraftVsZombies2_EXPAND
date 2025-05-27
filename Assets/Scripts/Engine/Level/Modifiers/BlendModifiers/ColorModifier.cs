using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class ColorModifier : PropertyModifier<Color>
    {
        public ColorModifier(PropertyKey<Color> propertyName, Color valueConst, int priority = 0) : this(propertyName, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha, valueConst, priority)
        {
        }
        public ColorModifier(PropertyKey<Color> propertyName, PropertyKey<Color> buffPropertyName, int priority = 0) : this(propertyName, BlendOperator.SrcAlpha, BlendOperator.OneMinusSrcAlpha, buffPropertyName, priority)
        {
        }
        public ColorModifier(PropertyKey<Color> propertyName, BlendOperator src, BlendOperator dest, Color valueConst, int priority = 0) : base(propertyName, valueConst, priority)
        {
            SrcOperator = src;
            DstOperator = dest;
        }

        public ColorModifier(PropertyKey<Color> propertyName, BlendOperator src, BlendOperator dest, PropertyKey<Color> buffPropertyName, int priority = 0) : base(propertyName, buffPropertyName, priority)
        {
            SrcOperator = src;
            DstOperator = dest;
        }
        public BlendOperator SrcOperator { get; private set; }
        public BlendOperator DstOperator { get; private set; }
        public static ColorModifier Multiply(PropertyKey<Color> propertyName, Color valueConst)
        {
            return new ColorModifier(propertyName, BlendOperator.DstColor, BlendOperator.Zero, valueConst);
        }
        public static ColorModifier Multiply(PropertyKey<Color> propertyName, PropertyKey<Color> buffPropertyName)
        {
            return new ColorModifier(propertyName, BlendOperator.DstColor, BlendOperator.Zero, buffPropertyName);
        }
        public static ColorModifier Override(PropertyKey<Color> propertyName, Color valueConst)
        {
            return new ColorModifier(propertyName, BlendOperator.One, BlendOperator.Zero, valueConst);
        }
        public static ColorModifier Override(PropertyKey<Color> propertyName, PropertyKey<Color> buffPropertyName)
        {
            return new ColorModifier(propertyName, BlendOperator.One, BlendOperator.Zero, buffPropertyName);
        }
        public override ModifierCalculator GetCalculator()
        {
            return CalculatorMap.colorCalculator;
        }
    }
}
