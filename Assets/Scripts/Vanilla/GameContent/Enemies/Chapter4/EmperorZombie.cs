﻿using System.Collections.Generic;
using MVZ2.GameContent.Armors;
using MVZ2.GameContent.Buffs.Enemies;
using MVZ2.GameContent.Detections;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Detections;
using MVZ2.Vanilla.Enemies;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using Tools;

namespace MVZ2.GameContent.Enemies
{
    [EntityBehaviourDefinition(VanillaEnemyNames.emperorZombie)]
    public class EmperorZombie : MeleeEnemy
    {
        public EmperorZombie(string nsp, string name) : base(nsp, name)
        {
            shieldDetector = new EmperorZombieShieldDetector(SHIELD_RADIUS);
        }

        public override void Init(Entity entity)
        {
            base.Init(entity);
            entity.EquipMainArmor(VanillaArmorID.emperorCrown);
            SetStateTimer(entity, new FrameTimer(CAST_COOLDOWN));
        }
        protected override int GetActionState(Entity enemy)
        {
            var state = base.GetActionState(enemy);
            if (state == VanillaEntityStates.WALK && IsCasting(enemy))
            {
                return STATE_CAST;
            }
            return state;
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetAnimationInt("HealthState", entity.GetHealthState(2));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);

            if (entity.IsDead)
                return;
            if (entity.State == VanillaEntityStates.ATTACK)
                return;
            var stateTimer = GetStateTimer(entity);
            if (entity.State == STATE_CAST)
            {
                stateTimer.Run(entity.GetAttackSpeed());
                if (stateTimer.Expired)
                {
                    EndCasting(entity);
                }
            }
            else
            {
                stateTimer.Run(entity.GetAttackSpeed());
                if (stateTimer.Expired)
                {
                    detectBuffer.Clear();
                    shieldDetector.DetectEntities(entity, detectBuffer);
                    if (detectBuffer.Count <= 0)
                    {
                        stateTimer.ResetTime(SHIELD_DETECT_TIME);
                    }
                    else
                    {
                        StartCasting(entity);
                        GrantShields(entity, detectBuffer.RandomTake(10, entity.RNG));
                    }
                }
            }
        }
        public override void PostDeath(Entity entity, DeathInfo info)
        {
            base.PostDeath(entity, info);
            if (entity.State == STATE_CAST)
            {
                EndCasting(entity);
            }
        }
        public static void SetCasting(Entity entity, bool timer) => entity.SetBehaviourField(ID, PROP_CASTING, timer);
        public static bool IsCasting(Entity entity) => entity.GetBehaviourField<bool>(ID, PROP_CASTING);
        public static void SetStateTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(ID, PROP_STATE_TIMER, timer);
        public static FrameTimer GetStateTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(ID, PROP_STATE_TIMER);

        private void StartCasting(Entity entity)
        {
            SetCasting(entity, true);
            entity.PlaySound(VanillaSoundID.divineShieldCast);
            var stateTimer = GetStateTimer(entity);
            stateTimer.ResetTime(CAST_TIME);
        }

        private void EndCasting(Entity entity)
        {
            SetCasting(entity, false);
            var stateTimer = GetStateTimer(entity);
            stateTimer.ResetTime(CAST_COOLDOWN);
        }

        private void GrantShields(Entity entity, IEnumerable<Entity> targets)
        {
            foreach (var target in targets)
            {
                target.AddBuff<DivineShieldBuff>();
            }
        }
        #region 常量
        public const int STATE_CAST = VanillaEntityStates.EMPEROR_ZOMBIE_CAST;
        public const int CAST_COOLDOWN = 150;
        public const int CAST_TIME = 30;
        public const int SHIELD_DETECT_TIME = 30;
        public const float SHIELD_RADIUS = 120;
        public static readonly NamespaceID ID = VanillaEnemyID.necromancer;
        public static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_STATE_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("StateTimer");
        public static readonly VanillaEntityPropertyMeta<bool> PROP_CASTING = new VanillaEntityPropertyMeta<bool>("Casting");
        private Detector shieldDetector;
        private List<Entity> detectBuffer = new List<Entity>();
        #endregion 常量
    }
}
