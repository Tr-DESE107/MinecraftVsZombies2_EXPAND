using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2Logic.Level;
using MVZ2.GameContent;
using MVZ2Logic.Games;
using MVZ2.Vanilla.Buffs;
using MVZ2Logic.Modding;
using PVZEngine;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.Vanilla
{
    public class StarshardSpawnImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.RegisterCallback(LevelCallbacks.PostEntityDeath, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
        }
        private void PostEnemyDeathCallback(Entity enemy, DamageInfo info)
        {
            var buffs = enemy.GetBuffs<StarshardCarrierBuff>();
            foreach (var buff in buffs)
            {
                enemy.Level.Spawn(VanillaPickupID.starshard, enemy.Position, enemy);
                buff.Remove();
            }
        }
    }
}
