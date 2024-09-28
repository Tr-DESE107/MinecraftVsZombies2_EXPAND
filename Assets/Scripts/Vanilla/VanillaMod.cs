using System;
using System.Linq;
using System.Reflection;
using MVZ2.Extensions;
using MVZ2.GameContent;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Stages;
using MVZ2.Modding;
using MVZ2.Resources;
using MVZ2.Save;
using MVZ2.Serialization;
using MVZ2.Vanilla.Save;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Level;
using Tools;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
            LoadStages();
            LoadFromAssemblies(new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) });

            AddCallback(LevelCallbacks.PostEntityTakeDamage, PostEntityTakeDamage);
            AddCallback(LevelCallbacks.PostEntityUpdate, ChangeLaneUpdate);
            AddCallback(LevelCallbacks.PostLevelClear, PostLevelClear);
            AddCallback(LevelCallbacks.PostEntityUpdate, CartUpdate, filter: EntityTypes.CART);
            AddCallback(BuiltinCallbacks.TalkAction, TalkAction);
        }
        public override void PostGameInit()
        {
            base.PostGameInit();
            SerializeHelper.RegisterClass<SerializableVanillaSaveData>();
        }
        public override ModSaveData CreateSaveData()
        {
            return new VanillaSaveData(spaceName);
        }
        public override ModSaveData LoadSaveData(string json)
        {
            var serializable = SerializeHelper.FromBson<SerializableVanillaSaveData>(json);
            return serializable.Deserialize();
        }
        protected void LoadFromAssemblies(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var definitionAttr = type.GetCustomAttribute<DefinitionAttribute>();
                    if (definitionAttr != null && !type.IsAbstract)
                    {
                        var name = definitionAttr.Name;
                        var constructor = type.GetConstructor(new Type[] { typeof(string), typeof(string) });
                        var definitionObj = constructor?.Invoke(new object[] { Namespace, name });
                        if (definitionObj is Definition def)
                        {
                            AddDefinition(def);
                        }
                        var seedEntityAttr = type.GetCustomAttribute<EntitySeedDefinitionAttribute>();
                        if (seedEntityAttr != null)
                        {
                            var seedDef = new EntitySeed(
                                Namespace,
                                name,
                                seedEntityAttr.Cost,
                                seedEntityAttr.RechargeID,
                                seedEntityAttr.TriggerActive,
                                seedEntityAttr.TriggerCost);
                            AddDefinition(seedDef);
                        }
                        var spawnDefAttr = type.GetCustomAttribute<SpawnDefinitionAttribute>();
                        if (spawnDefAttr != null)
                        {
                            var spawnDef = new SpawnDefinition(Namespace, name, spawnDefAttr.SpawnCost, new NamespaceID(Namespace, name));
                            AddDefinition(spawnDef);
                        }
                    }
                }
            }
        }

        private void LoadStages()
        {
            AddStage(new TutorialStage(spaceName, StageNames.tutorial));

            foreach (var stageMeta in Global.Game.GetModStageMetas(spaceName).Where(m => m.type == StageMeta.TYPE_NORMAL))
            {
                var stage = new ClassicStage(spaceName, StageNames.prologue);
                var meta = Global.Game.GetStageMeta(stage.GetID());
                if (meta != null)
                {
                    stage.SetProperty(StageProperties.TOTAL_FLAGS, meta.totalFlags);
                    stage.SetProperty(BuiltinStageProps.START_TALK, meta.startTalk);
                    stage.SetProperty(BuiltinStageProps.END_TALK, meta.endTalk);
                    stage.SetProperty(BuiltinStageProps.END_NOTE_ID, meta.endNote);
                    stage.SetLevelName(meta.name);

                    stage.SetProperty(BuiltinStageProps.START_CAMERA_POSITION, (int)meta.startCameraPosition);
                    stage.SetProperty(BuiltinStageProps.START_TRANSITION, meta.startTransition);
                    stage.SetSpawnEntries(meta.spawns);
                }
                AddStage(stage);
            }
        }
        private void AddStage(StageDefinition definition)
        {
            AddDefinition(definition);
        }
        private void PostEntityTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            if (armorResult != null && !armorResult.Effects.HasEffect(DamageEffects.MUTE))
            {
                var entity = armorResult.Entity;
                var shellDefinition = armorResult.ShellDefinition;
                entity.PlayHitSound(armorResult.Effects, shellDefinition);
            }
            if (bodyResult != null && !bodyResult.Effects.HasEffect(DamageEffects.MUTE))
            {
                var entity = bodyResult.Entity;
                var shellDefinition = bodyResult.ShellDefinition;
                entity.PlayHitSound(bodyResult.Effects, shellDefinition);
            }
        }
        private void ChangeLaneUpdate(Entity entity)
        {
            if (entity.Definition is not IChangeLaneEntity changeLane)
                return;
            if (!changeLane.IsChangingLane(entity))
                return;
            var targetLane = changeLane.GetChangeLaneTarget(entity);
            if (targetLane < 0 || targetLane > entity.Level.GetMaxLaneCount())
                return;
            var sourceLane = changeLane.GetChangeLaneSource(entity);

            float targetZ = entity.Level.GetEntityLaneZ(targetLane);
            bool passed;
            // Warp upwards.
            if (sourceLane > targetLane)
            {
                passed = entity.Pos.z >= targetZ - 0.03f;
            }
            // Warp downwards.
            else
            {
                passed = entity.Pos.z <= targetZ + 0.03f;
            }

            if (!passed)
            {
                Vector3 velocity = entity.Velocity;
                float warpSpeed = changeLane.GetChangeLaneSpeed(entity);

                // Warp upwards.
                if (sourceLane > targetLane)
                {
                    velocity.z = Mathf.Max(warpSpeed, entity.Velocity.z);
                }
                // Warp downwards.
                else
                {
                    velocity.z = Mathf.Min(-warpSpeed, entity.Velocity.z);
                }
                entity.Velocity = velocity;
            }
            else
            {
                if (Mathf.Abs(entity.Pos.z - targetZ) <= 0.05f)
                {
                    var pos = entity.Pos;
                    pos.z = targetZ;
                    entity.Pos = pos;
                }
                entity.StopChangingLane();
            }
        }
        private void PostLevelClear(LevelEngine level)
        {
            foreach (var cart in level.GetEntities(EntityTypes.CART))
            {
                if (cart.IsCartTriggered())
                    continue;
                cart.SetTurnToMoneyTimer(new FrameTimer(30 + 15 * (level.GetMaxLaneCount() - cart.GetLane())));
            }
        }
        private void CartUpdate(Entity entity)
        {
            FrameTimer timer = entity.GetTurnToMoneyTimer();
            if (timer == null)
                return;
            timer.Run();
            if (timer.Expired)
            {
                var level = entity.Level;
                var gemType = GemEffect.GemType.Ruby;
                if (level.Difficulty == LevelDifficulty.easy)
                {
                    gemType = GemEffect.GemType.Emerald;
                }
                var gemEffect = GemEffect.SpawnGemEffect(level, gemType, entity.Pos, entity, true);
                gemEffect.PlaySound(SoundID.points, 1 + (level.GetMaxLaneCount() - entity.GetLane() - 1) * 0.1f);
                level.ShowMoney();
                entity.Remove();
            }
        }
        private void TalkAction(ITalkSystem system, string cmd, string[] parameters)
        {
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            switch (cmd)
            {
                case "create_seventh_slot_form":
                    ShowSeventhSlotDialog(system);
                    break;
                case "create_tutorial_form":
                    ShowTutorialDialog(system);
                    break;
                case "try_buy_seventh_slot":
                    TryBuySeventhSlot(system);
                    break;

                case "start_tutorial":
                    level.ChangeStage(StageID.tutorial);
                    break;
                case "start_starshard_tutorial":
                    level.ChangeStage(StageID.starshard_tutorial);
                    break;
                case "start_trigger_tutorial":
                    level.ChangeStage(StageID.trigger_tutorial);
                    break;
            }
        }
        private void ShowSeventhSlotDialog(ITalkSystem system)
        {
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            if (level.StageID != StageID.halloween7)
            {
                Debug.LogError("尝试在非万圣夜场景创建第七卡槽对话框。");
                return;
            }

            var title = game.GetText(TextID.UI_PURCHASE);
            var desc = game.GetText(TextID.UI_CONFIRM_BUY_7TH_SLOT);
            var options = new string[]
            {
                game.GetText(TextID.UI_YES),
                game.GetText(TextID.UI_NO)
            };
            level.ShowDialog(title, desc, options, (index) =>
            {
                switch (index)
                {
                    case 0:
                        game.AddMoney(-750);
                        level.SetSeedPackCount(7);
                        game.SetBlueprintSlots(7);
                        system.StartSection(3);
                        break;
                    case 1:
                        system.StartSection(4);
                        break;
                }
            });
        }
        private void ShowTutorialDialog(ITalkSystem system)
        {
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            if (level.StageID != StageID.prologue)
            {
                Debug.LogError("尝试在非教程场景创建教程对话框。");
                return;
            }

            var title = game.GetText(TextID.UI_TUTORIAL);
            var desc = game.GetText(TextID.UI_CONFIRM_TUTORIAL);
            var options = new string[]
            {
                game.GetText(TextID.UI_YES),
                game.GetText(TextID.UI_NO)
            };
            level.ShowDialog(title, desc, options, (index) =>
            {
                switch (index)
                {
                    case 0:
                        system.StartSection(1);
                        break;
                    case 1:
                        system.StartSection(2);
                        break;
                }
            });
        }
        private void TryBuySeventhSlot(ITalkSystem system)
        {
            var game = Global.Game;
            if (!game.IsInLevel())
            {
                return;
            }
            LevelEngine level = game.GetLevel();
            level.ShowMoney();
            if (game.GetMoney() >= 750)
            {
                system.StartSection(1);
            }
            else
            {
                system.StartSection(2);
            }
        }
        public const string spaceName = "mvz2";
    }
}
