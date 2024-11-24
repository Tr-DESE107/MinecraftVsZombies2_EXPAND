using PVZEngine.Modifiers;

namespace MVZ2.Modifiers
{
    public static class ModifierHelper
    {
        public static void Init()
        {
            if (isInited)
                return;
            RegisterCalculator<BooleanModifier>(new BooleanCalculator());
            RegisterCalculator<StringModifier>(new StringCalculator());
            RegisterCalculator<NamespaceIDModifier>(new NamespaceIDCalculator());

            RegisterCalculator<IntModifier>(new IntCalculator());
            RegisterCalculator<FloatModifier>(new FloatCalculator());
            RegisterCalculator<Vector3Modifier>(new Vector3Calculator());

            RegisterCalculator<ColorModifier>(new ColorCalculator());

            isInited = true;
        }
        public static void RegisterCalculator<T>(ModifierCalculator calculator) where T : PropertyModifier
        {
            if (!ModifierMap.HasCalculator(typeof(T)))
                ModifierMap.AddCalculator<T>(calculator);
        }
        public static bool isInited { get; private set; } = false;
    }
}
