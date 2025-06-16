﻿using System.Collections.Generic;
using System.Linq;
using MVZ2.GameContent.Buffs.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Pickups;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Properties;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Level;
using PVZEngine;
using PVZEngine.Damages;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.SeedPacks;
using Tools;
using UnityEngine;

namespace MVZ2.GameContent.Contraptions
{
    [EntityBehaviourDefinition(VanillaContraptionNames.desirePot)]
    public class DesirePot : ContraptionBehaviour
    {
        public DesirePot(string nsp, string name) : base(nsp, name)
        {
        }
        public override void Init(Entity entity)
        {
            base.Init(entity);
            SetEvocationTimer(entity, new FrameTimer(EVOCATION_COOLDOWN));
        }
        protected override void UpdateAI(Entity entity)
        {
            base.UpdateAI(entity);
            if (entity.State == STATE_EVOKED)
            {
                EvokedUpdate(entity);
            }
        }
        protected override void UpdateLogic(Entity entity)
        {
            base.UpdateLogic(entity);
            entity.SetModelProperty("Evoked", entity.State == STATE_EVOKED);
            entity.SetModelProperty("DuplicatedCount", GetDuplicatedCount(entity));
        }

        protected override void OnEvoke(Entity entity)
        {
            base.OnEvoke(entity);

            var level = entity.Level;
            var selected = GetBlueprintsToCopy(entity);
            SpawnBlueprintPickups(entity, selected);

            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Reset();
            entity.State = STATE_EVOKED;
            entity.AddBuff<DesirePotHighlightBuff>();
            entity.PlaySound(VanillaSoundID.arcaneIntellect);
            entity.PlaySound(VanillaSoundID.desirePotEvocation);
        }
        private void EvokedUpdate(Entity entity)
        {
            var evocationTimer = GetEvocationTimer(entity);
            evocationTimer.Run();
            if (evocationTimer.Expired)
            {
                entity.AddBuff<DesirePotHighlightBuff>();
                entity.State = STATE_IDLE;
            }
        }
        private void SpawnBlueprintPickups(Entity entity, SeedPack[] selected)
        {
            var selectedCount = selected.Length;
            var minXSpeed = -3;
            var maxXSpeed = 3;

            var level = entity.Level;
            int drawnDesirePots = 0;
            int missDrawCount = 0;
            for (int i = 0; i < selectedCount; i++)
            {
                var seed = selected[i];
                if (seed == null)
                {
                    missDrawCount++;
                    continue;
                }
                var blueprintID = seed.GetDefinitionID();
                if (!NamespaceID.IsValid(blueprintID))
                {
                    missDrawCount++;
                    continue;
                }
                var spawnParams = entity.GetSpawnParams();
                spawnParams.SetProperty(BlueprintPickup.PROP_BLUEPRINT_ID, blueprintID);
                spawnParams.SetProperty(BlueprintPickup.PROP_COMMAND_BLOCK, seed.IsCommandBlock());
                var pickup = entity.Spawn(VanillaPickupID.blueprintPickup, entity.GetCenter(), spawnParams);

                if (seed is ClassicSeedPack)
                {
                    seed.SetStartRecharge(false);
                    seed.ResetRecharge();
                }

                float xSpeed = 0;
                if (selectedCount > 1)
                {
                    xSpeed = minXSpeed + i / (float)(selectedCount - 1) * (maxXSpeed - minXSpeed);
                }
                var vel = new Vector3(xSpeed, 7, 0);
                pickup.Velocity = vel;

                if (blueprintID == VanillaBlueprintID.FromEntity(VanillaContraptionID.desirePot))
                {
                    drawnDesirePots++;
                }
            }


            if (missDrawCount > 0)
            {
                if (level.IsConveyorMode())
                {
                    level.ShowAdvice(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_NO_CARDS_DRAWN_CONVEYOR, 0, 150);
                }
                else
                {
                    float fatigueDamageSum = 0;
                    for (int i = 0; i < missDrawCount; i++)
                    {
                        fatigueDamageSum += Fatigue(entity);
                    }
                    level.ShakeScreen(10, 0, 15);
                    var sum = Mathf.FloorToInt(fatigueDamageSum);
                    level.ShowAdvicePlural(VanillaStrings.CONTEXT_ADVICE, VanillaStrings.ADVICE_NO_CARDS_DRAWN, sum, 0, 150, sum.ToString());
                    entity.PlaySound(VanillaSoundID.fatigue);
                }
            }

            if (drawnDesirePots >= 2)
            {
                Global.Game.Unlock(VanillaUnlockID.overdraw);
                Global.Game.SaveToFile(); // 完成成就后保存游戏。
            }
        }
        private SeedPack[] GetBlueprintsToCopy(Entity entity)
        {
            var level = entity.Level;
            IEnumerable<SeedPack> heldBlueprints;
            if (level.IsConveyorMode())
            {
                heldBlueprints = level.GetAllConveyorSeedPacks();
            }
            else
            {
                heldBlueprints = level.GetAllSeedPacks().Where(e => e != null && e.IsCharged());
            }
            var sourceBlueprints = heldBlueprints.TakeLast(EVOCATION_CARD_COUNT);

            SeedPack[] pile = new SeedPack[EVOCATION_CARD_COUNT];
            var count = sourceBlueprints.Count();
            for (int i = 0; i < pile.Length; i++)
            {
                if (i >= count)
                    continue;
                pile[i] = sourceBlueprints.ElementAt(i);
            }
            return pile;
        }
        private float Fatigue(Entity entity)
        {
            var level = entity.Level;
            var damage = GetFatigueDamage(level);
            damage += FATIGUE_INCREAMENT;
            SetFatigueDamage(level, damage);
            entity.Level.AddEnergy(-damage);
            return damage;
        }

        public static void DuplicateStarshard(Entity pot)
        {
            pot.Spawn(VanillaPickupID.starshard, pot.GetCenter());
            var count = GetDuplicatedCount(pot);
            count++;
            SetDuplicatedCount(pot, count);
            if (count >= MAX_DUPLICATED_COUNT)
            {
                var effects = new DamageEffectList(VanillaDamageEffects.SELF_DAMAGE);
                pot.Die(effects, pot);
            }
        }

        public static FrameTimer GetEvocationTimer(Entity entity) => entity.GetBehaviourField<FrameTimer>(PROP_EVOCATION_TIMER);
        public static void SetEvocationTimer(Entity entity, FrameTimer timer) => entity.SetBehaviourField(PROP_EVOCATION_TIMER, timer);

        public static int GetDuplicatedCount(Entity entity) => entity.GetBehaviourField<int>(PROP_DUPLICATED_COUNT);
        public static void SetDuplicatedCount(Entity entity, int value) => entity.SetBehaviourField(PROP_DUPLICATED_COUNT, value);

        public static float GetFatigueDamage(LevelEngine level) => level.GetBehaviourField<float>(PROP_FATIGUE_DAMAGE);
        public static void SetFatigueDamage(LevelEngine level, float value) => level.SetBehaviourField(PROP_FATIGUE_DAMAGE, value);



        public const int EVOCATION_COOLDOWN = 90;
        public const int EVOCATION_CARD_COUNT = 2;
        public const int FATIGUE_INCREAMENT = 25;
        public const int DETECT_INTERVAL = 10;
        public const int MAX_DUPLICATED_COUNT = 3;
        public const int STATE_IDLE = VanillaEntityStates.IDLE;
        public const int STATE_EVOKED = VanillaEntityStates.CONTRAPTION_SPECIAL;
        public const string PROP_REGION = VanillaContraptionNames.desirePot;
        [LevelPropertyRegistry(PROP_REGION)]
        private static readonly VanillaLevelPropertyMeta<float> PROP_FATIGUE_DAMAGE = new VanillaLevelPropertyMeta<float>("FatigueDamage");
        private static readonly VanillaEntityPropertyMeta<int> PROP_DUPLICATED_COUNT = new VanillaEntityPropertyMeta<int>("duplicated_count");
        private static readonly VanillaEntityPropertyMeta<FrameTimer> PROP_EVOCATION_TIMER = new VanillaEntityPropertyMeta<FrameTimer>("EvocationTimer");
    }
}
