using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.HostIMP)]
    public class HostIMP : Imp
    {
        public HostIMP(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.InflictRegenerationBuff(4f, 60000);
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
            entity.SetAnimationBool("HasBoat", entity.HasBuff<BoatBuff>());
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // µÙ¬‰ÀÈ¥¨µÙ¬‰ŒÔ
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity);
                effect.Velocity = new Vector3(effect.RNG.NextFloat() * 20 - 10, 5, 0);
                effect.ChangeModel(VanillaModelID.boatItem);
                effect.SetDisplayScale(entity.GetDisplayScale());
            }
        }
    }
}
