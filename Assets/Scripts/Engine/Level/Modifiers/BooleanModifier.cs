using PVZEngine.LevelManagement;

namespace PVZEngine.Modifiers
{
    public class StringModifier : PropertyModifier<string>
    {
        public StringModifier(string propertyName, ModifyOperator op, string valueConst) : base(propertyName, op, valueConst)
        {
        }
        public static StringModifier FromPropertyName(string propertyName, ModifyOperator op, string buffPropertyName)
        {
            return new StringModifier(propertyName, op, null)
            {
                UsingBuffPropertyName = buffPropertyName
            };
        }

        public override string CalculatePropertyGeneric(Buff buff, string value)
        {
            return GetValueGeneric(buff);
        }
    }
}
