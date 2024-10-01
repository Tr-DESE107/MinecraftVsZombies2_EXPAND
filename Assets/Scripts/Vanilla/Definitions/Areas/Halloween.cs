using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(VanillaAreaNames.halloween)]
    public class Halloween : AreaDefinition
    {
        public Halloween(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinAreaProps.DOOR_Z, 160f);
            SetProperty(EngineAreaProps.CART_REFERENCE, VanillaCartID.pumpkinCarriage);
            SetProperty(BuiltinLevelProps.MUSIC_ID, MusicID.halloween);
            SetProperty(BuiltinAreaProps.NIGHT_VALUE, 0.5f);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(VanillaGridID.grass);
            }
        }
        public override float GetGroundY(float x, float z)
        {
            switch (x)
            {
                case > 185:
                    return 0;
                case > 175:
                    // 地面和第一层的交界处
                    return Mathf.Lerp(0, 48, (185 - x) / 10);
                case > 140:
                    return 48;
                case > 130:
                    // 第一层和第二层的交界处
                    return Mathf.Lerp(48, 96, (140 - x) / 10);
                case > 95:
                    return 96;
                case > 85:
                    // 第二层和第三层的交界处
                    return Mathf.Lerp(96, 144, (95 - x) / 10);
                default:
                    return 144;
            }
        }
    }
}