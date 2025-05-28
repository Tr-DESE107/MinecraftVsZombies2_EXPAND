namespace PVZEngine.Modifiers
{
    public static class CalculatorMap
    {
        public static readonly ModifierCalculator booleanCalculator = new BooleanCalculator();
        public static readonly ModifierCalculator stringCalculator = new StringCalculator();
        public static readonly ModifierCalculator namespaceIDCalculator = new NamespaceIDCalculator();
        public static readonly ModifierCalculator namespaceIDArrayCalculator = new NamespaceIDArrayCalculator();

        public static readonly ModifierCalculator intCalculator = new IntCalculator();
        public static readonly ModifierCalculator floatCalculator = new FloatCalculator();
        public static readonly ModifierCalculator vector3Calculator = new Vector3Calculator();

        public static readonly ModifierCalculator colorCalculator = new ColorCalculator();
    }
}
