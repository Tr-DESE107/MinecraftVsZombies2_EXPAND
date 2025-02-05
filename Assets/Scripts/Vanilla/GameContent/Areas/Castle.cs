using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Buffs.Level;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Level;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Areas
{
    [Definition(VanillaAreaNames.castle)]
    public class Castle : AreaDefinition
    {
        public Castle(string nsp, string name) : base(nsp, name)
        {
        }
        public override void PostHugeWaveEvent(LevelEngine level)
        {
            base.PostHugeWaveEvent(level);
            var x = level.GetEnemySpawnX();
            var z = level.GetEntityLaneZ(level.GetMaxLaneCount() / 2);
            var y = level.GetGroundY(x, z);
            var pos = new Vector3(x, y, z);
            level.Spawn(VanillaEnemyID.reverseSatellite, pos, null);
        }
        public override float GetGroundY(LevelEngine level, float x, float z)
        {
            if (x < 660)
            {
                return Mathf.Lerp(80, 0, (x - 260) / 400);
            }
            if (x > 1060)
            {
                return Mathf.Lerp(0, 80, (x - 1060) / 400);
            }
            return base.GetGroundY(level, x, z);
        }
        private static readonly NamespaceID ID = VanillaAreaID.castle;
    }
}