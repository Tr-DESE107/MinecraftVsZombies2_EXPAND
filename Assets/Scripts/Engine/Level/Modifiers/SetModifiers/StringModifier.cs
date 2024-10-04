using PVZEngine.Level;

namespace PVZEngine.Modifiers
{
    public class StringModifier : PropertyModifier<string>
    {
        public StringModifier(string propertyName, string valueConst) : base(propertyName, null)
        {
            ConstValue = valueConst;
        }
        public static StringModifier FromPropertyName(string propertyName, string buffPropertyName)
        {
            return new StringModifier(propertyName, null)
            {
                UsingBuffPropertyName = buffPropertyName
            };
        }
    }
}
