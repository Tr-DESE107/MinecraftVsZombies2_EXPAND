using System.Collections.Generic;
using System.Linq;
using MukioI18n;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.Games;
using MVZ2.Modding;
using MVZ2.Vanilla.Buffs;
using PVZEngine;
using PVZEngine.Level;

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
                enemy.Level.Spawn(PickupID.starshard, enemy.Position, enemy);
                enemy.RemoveBuff(buff);
            }
        }
    }
}
