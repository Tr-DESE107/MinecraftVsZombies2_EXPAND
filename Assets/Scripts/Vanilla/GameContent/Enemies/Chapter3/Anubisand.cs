﻿using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.anubisand)]
    public class Anubisand : StateEnemy
    {
        public Anubisand(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetSummonTimer(entity, new FrameTimer(SUMMON_INTERVAL));
            var buff = entity.AddBuff<FlyBuff>();
            buff.SetProperty(FlyBuff.PROP_TARGET_HEIGHT, 80f);
        }
        protected override void UpdateAI(Entity enemy)
        {
            base.UpdateAI(enemy);
            if (enemy.IsDead)
                return;
            var timer = GetSummonTimer(enemy);
            timer.Run(enemy.GetAttackSpeed());
            if (timer.Expired)
            {
                timer.Reset();
                var offset = SUMMON_OFFSET;
                offset.x *= enemy.GetFacingX();
                var sand = enemy.SpawnWithParams(VanillaEnemyID.soulsand, enemy.Position + offset);
                sand.AddBuff<SoulsandSummonedBuff>();
                enemy.PlaySound(VanillaSoundID.cave);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            var timer = GetSummonTimer(entity);
            var blend = GetBlackholeBlend(entity);
            if (timer.Frame <= SUMMON_INTERVAL / 2)
            {
                blend = 1 - timer.Frame / (float)(SUMMON_INTERVAL * 0.5f);
            }
            else
            {
                blend -= 1 / 15f;
            }
            SetBlackholeBlend(entity, blend);
            entity.SetAnimationFloat("BlackholeBlend", blend);
            entity.SetModelDamagePercent();
        }
        public static FrameTimer GetSummonTimer(Entity enemy) => enemy.GetBehaviourField<FrameTimer>(ID, FIELD_SUMMON_TIMER);
        public static void SetSummonTimer(Entity enemy, FrameTimer value) => enemy.SetBehaviourField(ID, FIELD_SUMMON_TIMER, value);

        public static float GetBlackholeBlend(Entity enemy) => enemy.GetBehaviourField<float>(ID, FIELD_BLACKHOLE_BLEND);
        public static void SetBlackholeBlend(Entity enemy, float value) => enemy.SetBehaviourField(ID, FIELD_BLACKHOLE_BLEND, value);

        public static readonly VanillaEntityPropertyMeta<FrameTimer> FIELD_SUMMON_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("SummonTimer");
        public static readonly VanillaEntityPropertyMeta<float> FIELD_BLACKHOLE_BLEND = new VanillaEntityPropertyMeta<float>("BlackholeBlend");
        public const int SUMMON_INTERVAL = 180;
        public static readonly Vector3 SUMMON_OFFSET = new Vector3(30, 80, 0);
        public static readonly NamespaceID ID = VanillaEnemyID.anubisand;
    }
}
