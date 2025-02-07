using MVZ2.GameContent.Effects;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [AreaDefinition(VanillaAreaNames.day)]
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