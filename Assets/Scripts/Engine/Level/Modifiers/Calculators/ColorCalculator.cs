using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine.Modifiers
{
    public class ColorCalculator : ModifierCalculator<Color, ColorModifier>
    {
        public override Color CalculateGeneric(Color value, IEnumerable<BuffModifierItem> modifiers)
        {
            foreach (var item in modifiers)
            {
                var buff = item.buff;
                var modi = item.modifier;
                if (modi is not ColorModifier modifier)
                    continue;
                var src = modifier.GetModifierValueGeneric(buff);
                var dst = value;
                var srcOp = modifier.SrcOperator;
                var dstOp = modifier.DstOperator;


                for (int i = 0; i < 4; i++)
                {
                    value[i] = src[i] * GetBlendedComponent(src, dst, srcOp, i) + dst[i] * GetBlendedComponent(src, dst, dstOp, i);
                }
            }
            return value;
        }
        private float GetBlendedComponent(Color src, Color dst, BlendOperator op, int compIndex)
        {
            switch (op)
            {
                case BlendOperator.One:
                    return 1;
                case BlendOperator.Zero:
                    return 0;
                case BlendOperator.SrcAlpha:
                    return src.a;
                case BlendOperator.DstAlpha:
                    return dst.a;
                case BlendOperator.OneMinusSrcAlpha:
                    return 1 - src.a;
                case BlendOperator.OneMinusDstAlpha:
                    return 1 - dst.a;
                case BlendOperator.SrcColor:
                    return src[compIndex];
                case BlendOperator.DstColor:
                    return dst[compIndex];
                case BlendOperator.OneMinusSrcColor:
                    return 1 - src[compIndex];
                case BlendOperator.OneMinusDstColor:
                    return 1 - dst[compIndex];
            }
            return 0;
        }
    }
}
