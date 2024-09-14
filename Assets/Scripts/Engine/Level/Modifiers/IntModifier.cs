using PVZEngine.Level;

namespace PVZEngine.Modifiers
{
    public class IntModifier : PropertyModifier<int>
    {
        public IntModifier(string propertyName, ModifyOperator op, int valueConst) : base(propertyName, op, valueConst)
        {
        }

        public IntModifier(string propertyName, ModifyOperator op, string buffPropertyName) : base(propertyName, op, buffPropertyName)
        {
        }

        public override int CalculatePropertyGeneric(Buff buff, int value)
        {
            switch (Operator)
            {
                case ModifyOperator.Add:
                    return value + GetValueGeneric(buff);
                case ModifyOperator.Multiply:
                    return value * GetValueGeneric(buff);
                case ModifyOperator.Average:
                    return (value + GetValueGeneric(buff)) / 2;
                case ModifyOperator.Set:
                    return GetValueGeneric(buff);
            }
            return value;
        }
    }
}
