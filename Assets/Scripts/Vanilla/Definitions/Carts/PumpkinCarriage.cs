using MVZ2.Vanilla;
using UnityEngine;

namespace MVZ2.GameContent.Carts
{
    [Definition(VanillaCartNames.pumpkinCarriage)]
    public class PumpkinCarriage : VanillaCart
    {
        public PumpkinCarriage(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinCartProps.CART_TRIGGER_SOUND, SoundID.horseAngry);
            SetProperty(VanillaEntityProps.IS_LIGHT_SOURCE, true);
            SetProperty(VanillaEntityProps.LIGHT_RANGE, Vector2.one * 80f);
            SetProperty(VanillaEntityProps.LIGHT_COLOR, new Color(0, 1, 0, 1));
        }
    }
}