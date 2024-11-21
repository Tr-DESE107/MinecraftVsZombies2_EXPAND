using MVZ2.GameContent.Effects;
using MVZ2.Vanilla;
using MVZ2Logic.Audios;
using MVZ2Logic.Level;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(VanillaAreaNames.day)]
    public class Day : AreaDefinition
    {
        public Day(string nsp, string name) : base(nsp, name)
        {
            SetProperty(BuiltinAreaProps.DOOR_Z, 240f);
            SetProperty(EngineAreaProps.CART_REFERENCE, VanillaCartID.minecart);
            SetProperty(BuiltinLevelProps.MUSIC_ID, MusicID.day);
            for (int i = 0; i < 45; i++)
            {
                grids.Add(VanillaGridID.grass);
            }
        }
        public override void Setup(LevelEngine level)
        {
            level.Spawn<Miner>(new Vector3(600, 0, 60), null);
        }
    }
}