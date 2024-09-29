using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(AreaNames.halloween)]
    public class Halloween : AreaDefinition
    {
        public Halloween(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinAreaProps.DOOR_Z, 160f);
            SetProperty(AreaProperties.CART_REFERENCE, CartID.pumpkinCarriage);
            SetProperty(BuiltinLevelProps.MUSIC_ID, MusicID.halloween);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(GridID.grass);
            }
        }
    }
}