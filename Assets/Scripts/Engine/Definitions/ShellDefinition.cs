using System.Collections.Generic;

namespace PVZEngine
{
    public class ShellDefinition : Definition
    {
        public void EvaluateDamage(DamageInfo damageInfo)
        {
            foreach (var op in operators)
            {
                op.Operate(damageInfo);
            }
        }
        private List<IDamageOperator> operators = new List<IDamageOperator>();

    }
}
