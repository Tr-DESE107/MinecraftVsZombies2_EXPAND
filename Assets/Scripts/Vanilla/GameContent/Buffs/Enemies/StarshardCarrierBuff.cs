﻿using System.Collections.Generic;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Pickups;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs.Enemies
{
    [BuffDefinition(VanillaBuffNames.starshardCarrier)]
    public class StarshardCarrierBuff : BuffDefinition
    {
        public StarshardCarrierBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new ColorModifier(EngineEntityProps.COLOR_OFFSET, PROP_COLOR_OFFSET));
            AddTrigger(VanillaLevelCallbacks.ENEMY_DROP_REWARDS, PostEnemyDropRewardsCallback);
        }

        public override void PostAdd(Buff buff)
        {
            base.PostAdd(buff);
            UpdateColorOffset(buff);
            UpdateDesirePot(buff);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            UpdateColorOffset(buff);
        }
        private void UpdateDesirePot(Buff buff)
        {
            var entity = buff.GetEntity();
            if (entity == null || !entity.IsVulnerableEntity())
                return;

            desirePotBuffer.Clear();
            entity.Level.FindEntitiesNonAlloc(e => e.IsEntityOf(VanillaContraptionID.desirePot), desirePotBuffer);
            foreach (var pot in desirePotBuffer)
            {
                var lump = pot.Spawn(VanillaEffectID.desireLump, entity.GetCenter());
                lump.SetParent(pot);
                pot.PlaySound(VanillaSoundID.shadowCast);
            }
        }
        private void UpdateColorOffset(Buff buff)
        {
            var time = buff.GetProperty<int>(PROP_TIME);
            time++;
            time %= MAX_TIME;
            var alpha = 1 - (Mathf.Cos((float)time / MAX_TIME * 360 * Mathf.Deg2Rad) + 1) / 2;
            alpha *= 0.8f;
            buff.SetProperty(PROP_COLOR_OFFSET, new Color(0, 1, 0, alpha));
            buff.SetProperty(PROP_TIME, time);
        }
        private void PostEnemyDropRewardsCallback(EntityCallbackParams param, CallbackResult result)
        {
            var enemy = param.entity;
            var buffs = enemy.GetBuffs<StarshardCarrierBuff>();
            foreach (var buff in buffs)
            {
                enemy.Level.Spawn(VanillaPickupID.starshard, enemy.Position, enemy);
                buff.Remove();
            }
        }
        public const int MAX_TIME = 60;
        public static readonly VanillaBuffPropertyMeta<int> PROP_TIME = new VanillaBuffPropertyMeta<int>("Time");
        public static readonly VanillaBuffPropertyMeta<Color> PROP_COLOR_OFFSET = new VanillaBuffPropertyMeta<Color>("ColorOffset");
        private List<Entity> desirePotBuffer = new List<Entity>();
    }
}
