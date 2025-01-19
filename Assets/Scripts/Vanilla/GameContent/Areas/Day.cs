using MVZ2.GameContent.Carts;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Grids;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using PVZEngine;
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
        }
        public override void Setup(LevelEngine level)
        {
            level.Spawn(VanillaEffectID.miner, new Vector3(600, 0, 60), null);
        }
    }
}