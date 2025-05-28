﻿using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine.Buffs;
using PVZEngine.Callbacks;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Modifiers;
using UnityEngine;

namespace MVZ2.GameContent.Buffs
{
    [BuffDefinition(VanillaBuffNames.iZombieAttackBooster)]
    public class IZombieAttackBoosterBuff : BuffDefinition
    {
        public IZombieAttackBoosterBuff(string nsp, string name) : base(nsp, name)
        {
            AddModifier(new FloatModifier(VanillaEntityProps.ATTACK_SPEED, NumberOperator.Multiply, PROP_ATTACK_SPEED_MULTIPLIER));
            AddTrigger(VanillaLevelCallbacks.POST_ENTITY_TAKE_DAMAGE, PostEnemyTakeDamageCallback, filter: EntityTypes.ENEMY);
        }
        public override void PostUpdate(Buff buff)
        {
            base.PostUpdate(buff);
            var attackTime = GetAttackTime(buff);
            var entity = buff.GetEntity();
            if (entity != null && entity.State == VanillaEntityStates.ATTACK)
            {
                attackTime++;
            }
            else
            {
                attackTime = 0;
            }
            SetAttackTime(buff, attackTime);
            float speedMultiplier = 1;
            if (attackTime > ATTACK_UP_TIME_START)
            {
                var t = (attackTime - ATTACK_UP_TIME_START) / (float)(ATTACK_UP_TIME_END - ATTACK_UP_TIME_START);
                speedMultiplier = Mathf.Lerp(ATTACK_UP_MULTIPLIER_START, ATTACK_UP_MULTIPLIER_END, t);
            }
            SetAttackSpeedMultiplier(buff, speedMultiplier);
        }
        private void PostEnemyTakeDamageCallback(VanillaLevelCallbacks.PostTakeDamageParams param, CallbackResult result)
        {
            var output = param.output;
            if (output == null)
                return;
            var entity = output.Entity;
            if (entity == null)
                return;
            var buffs = entity.GetBuffs<IZombieAttackBoosterBuff>();
            foreach (var buff in buffs)
            {
                SetAttackTime(buff, 0);
            }
        }
        public static int GetAttackTime(Buff buff) => buff.GetProperty<int>(PROP_ATTACK_TIME);
        public static void SetAttackTime(Buff buff, int value) => buff.SetProperty(PROP_ATTACK_TIME, value);
        public static float GetAttackSpeedMultiplier(Buff buff) => buff.GetProperty<float>(PROP_ATTACK_SPEED_MULTIPLIER);
        public static void SetAttackSpeedMultiplier(Buff buff, float value) => buff.SetProperty(PROP_ATTACK_SPEED_MULTIPLIER, value);
        public const int ATTACK_UP_TIME_START = 150;
        public const int ATTACK_UP_TIME_END = 300;
        public const float ATTACK_UP_MULTIPLIER_START = 1;
        public const float ATTACK_UP_MULTIPLIER_END = 5;
        public static readonly VanillaBuffPropertyMeta<int> PROP_ATTACK_TIME = new VanillaBuffPropertyMeta<int>("attackTime");
        public static readonly VanillaBuffPropertyMeta<float> PROP_ATTACK_SPEED_MULTIPLIER = new VanillaBuffPropertyMeta<float>("attackSpeedMultiplier");
    }
}
