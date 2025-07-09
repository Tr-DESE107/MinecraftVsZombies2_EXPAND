﻿using MVZ2.GameContent.Buffs;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Models;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.imp)]
    public class Imp : MeleeEnemy
    {
        public Imp(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            if (entity.Level.IsIZombie())
            {
                entity.AddBuff<IZombieImpBuff>();
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelDamagePercent();
            entity.SetModelProperty("HasBoat", entity.HasBuff<BoatBuff>());
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.HasBuff<BoatBuff>())
            {
                entity.RemoveBuffs<BoatBuff>();
                // 掉落碎船掉落物
                var effect = entity.Level.Spawn(VanillaEffectID.brokenArmor, entity.GetCenter(), entity);
                effect.Velocity = new Vector3(effect.RNG.NextFloat() * 20 - 10, 5, 0);
                effect.ChangeModel(VanillaModelID.boatItem);
                effect.SetDisplayScale(entity.GetDisplayScale());
            }
        }
    }
}
