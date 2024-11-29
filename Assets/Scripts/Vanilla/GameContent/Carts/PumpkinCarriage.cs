using MVZ2.Vanilla;
using MVZ2.Vanilla.Entities;

namespace MVZ2.GameContent.Carts
{
    [Definition(VanillaCartNames.pumpkinCarriage)]
    public class PumpkinCarriage : CartBehaviour
    {
        public PumpkinCarriage(string nsp, string name) : base(nsp, name)
        {
        }
    }
}