﻿using MVZ2.Vanilla.Entities;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.soulsand)]
    public class Soulsand : EnemyBehaviour
    {
        public Soulsand(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.Timeout = entity.GetMaxTimeout();
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);

            if (entity.Velocity.y != 0)
            {
                entity.AddFragmentTickDamage(Mathf.Abs(entity.Velocity.y));
            }
            entity.SetModelDamagePercent();
            if (entity.Timeout >= 0)
            {
                entity.Timeout--;
                if (entity.Timeout <= 0)
                {
                    entity.Die(entity);
                }
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            entity.Remove();
        }
    }
}
