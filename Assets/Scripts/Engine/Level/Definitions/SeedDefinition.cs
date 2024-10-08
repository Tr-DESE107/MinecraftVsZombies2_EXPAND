using PVZEngine.Base;
using PVZEngine.Level;

namespace PVZEngine.Definitions
{
    public class SeedDefinition : Definition
    {
        public SeedDefinition(string nsp, string name) : base(nsp, name)
        {
            SetProperty(EngineSeedProps.RECHARGE_SPEED, 1);
        }
    }
}
