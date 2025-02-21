using MVZ2.GameContent.Bosses;
using MVZ2.GameContent.Buffs.Armors;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Entities;
using PVZEngine.Armors;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.seijaMesmerizer)]
    public class SeijaMesmerizerBuff : BuffDefinition
    {
        public SeijaMesmerizerBuff(string nsp, string name) : base(nsp, name)
        {
            AddTrigger(LevelCallbacks.POST_ENTITY_DEATH, PostEntityDeathCallback);
        }
        private void PostEntityDeathCallback(Entity enemy, DeathInfo info)
        {
            if (!enemy.HasBuff<SeijaMesmerizerBuff>())
                return;
            foreach (var seija in enemy.Level.FindEntities(VanillaBossID.seija))
            {
                if (seija.IsDead)
                    continue;
                seija.Die();
            }
        }
    }
}
