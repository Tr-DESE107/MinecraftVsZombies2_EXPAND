using PVZEngine;

namespace MVZ2.Vanilla.Properties
{
    public class VanillaBuffPropertyMeta : PropertyMeta
    {
        public VanillaBuffPropertyMeta(string name) : base(name)
        {
        }
        public static readonly VanillaBuffPropertyMeta REGEN_HEAL_AMOUNT = new VanillaBuffPropertyMeta("RegenHealAmount");
        public static readonly VanillaBuffPropertyMeta REGEN_TIMEOUT = new VanillaBuffPropertyMeta("RegenTimeout");


    }
}
