using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using PVZEngine;
using PVZEngine.Buffs;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.gemCarrier)]
    public class GemCarrierBuff : BuffDefinition
    {
        public GemCarrierBuff(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, PostEnemyDropRewardsCallback);
        }
        private void PostEnemyDropRewardsCallback(Entity enemy)
        {
            var buffs = enemy.GetBuffs<RedstoneCarrierBuff>();
            foreach (var buff in buffs)
            {
                var weights = gemWeights.Select(g => g.weight).ToArray();
                var index = enemy.DropRNG.WeightedRandom(weights);
                var gemID = gemWeights[index].id;
                enemy.Produce(gemID);
                buff.Remove();
            }
        }
        private static readonly List<(NamespaceID id, float weight)> gemWeights = new List<(NamespaceID, float)>()
        {
            ( VanillaPickupID.emerald, 100 ),
            ( VanillaPickupID.ruby, 20 ),
            ( VanillaPickupID.diamond, 1 ),
        };
    }
}
