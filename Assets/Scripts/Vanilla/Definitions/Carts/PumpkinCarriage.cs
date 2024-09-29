using MVZ2.Vanilla;

namespace MVZ2.GameContent.Carts
{
    [Definition(CartNames.pumpkinCarriage)]
    public class PumpkinCarriage : VanillaCart
    {
        public PumpkinCarriage(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinCartProps.CART_TRIGGER_SOUND, SoundID.horseAngry);
        }
    }
}