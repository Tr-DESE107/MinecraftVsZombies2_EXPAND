using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Pickups;
using MVZ2Logic.Modding;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;

namespace MVZ2.GameContent.Implements
{
    public class StarshardSpawnImplements : VanillaImplements
    {
        public override void Implement(Mod mod)
        {
            mod.AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEnemyDeathCallback, filter: EntityTypes.ENEMY);
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
