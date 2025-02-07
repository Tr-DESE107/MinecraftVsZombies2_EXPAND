using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Callbacks;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.redstoneCarrier)]
    public class RedstoneCarrierBuff : BuffDefinition
    {
        public RedstoneCarrierBuff(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, PostEnemyDropRewardsCallback);
        }
        private void PostEnemyDropRewardsCallback(Entity enemy)
        {
            var buffs = enemy.GetBuffs<RedstoneCarrierBuff>();
            foreach (var buff in buffs)
            {
                for (int i = 0; i < 3; i++)
                {
                    var redstone = enemy.Level.Spawn(VanillaPickupID.redstone, enemy.Position + Vector3.up * 10, enemy);
                    redstone.Velocity = new Vector3(redstone.RNG.Next(-1f, 1f), 2, 0);
                }
                buff.Remove();
            }
        }
    }
}
