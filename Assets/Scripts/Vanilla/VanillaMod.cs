using System;
using System.Linq;
using System.Reflection;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Damages;
using MVZ2.GameContent.Difficulties;
using MVZ2.GameContent.Effects;
using MVZ2.GameContent.Implements;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla.Audios;
using MVZ2.Vanilla.Callbacks;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Entities;
using MVZ2Logic.Grids;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using MVZ2Logic.Saves;
using MVZ2Logic.Talk;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Callbacks;
using PVZEngine.Damages;
using PVZEngine.Definitions;
using PVZEngine.Entities;
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
            LoadEntityProperties();

            RegisterCallback(VanillaLevelCallbacks.PostEntityTakeDamage, PostEntityTakeDamage);
            RegisterCallback(LevelCallbacks.PostEntityUpdate, ChangeLaneUpdate);
            RegisterCallback(LevelCallbacks.PostLevelClear, PostLevelClear);
            RegisterCallback(LevelCallbacks.PostEntityUpdate, CartUpdate, filter: EntityTypes.CART);
            RegisterCallback(VanillaCallbacks.TalkAction, TalkAction);

            ImplementCallbacks(new GemStageImplements());
            ImplementCallbacks(new StarshardSpawnImplements());
            ImplementCallbacks(new EntityImplements());
            ImplementCallbacks(new DifficultyImplements());
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
                            spawnDef.SetProperty(VanillaSpawnProps.PREVIEW_COUNT, spawnDefAttr.PreviewCount);
                            AddDefinition(spawnDef);
                        }
                    }
                }
            }
        }
        private void ImplementCallbacks(VanillaImplements implements)
        {
            implements.Implement(this);
        }
        private void LoadStages()
        {
            AddStage(new TutorialStage(spaceName, VanillaStageNames.tutorial));
            AddStage(new StarshardTutorialStage(spaceName, VanillaStageNames.starshardTutorial));

            foreach (var meta in Global.Game.GetModStageMetas(spaceName).Where(m => m.type == StageMeta.TYPE_NORMAL))
            {
                if (meta == null)
                    continue;
                var stage = new ClassicStage(spaceName, meta.id);
                stage.SetSpawnEntries(meta.spawns);
                AddStage(stage);
            }
            foreach (var meta in Global.Game.GetModStageMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var stage = GetDefinition<StageDefinition>(new NamespaceID(spaceName, meta.id));
                if (stage == null)
                    continue;
                stage.SetLevelName(meta.name);
                stage.SetDayNumber(meta.dayNumber);

                stage.SetProperty(VanillaStageProps.START_TALK, meta.startTalk);
                stage.SetProperty(VanillaStageProps.END_TALK, meta.endTalk);
                stage.SetProperty(VanillaStageProps.MAP_TALK, meta.mapTalk);

                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_MODEL, meta.clearPickupModel);
                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_BLUEPRINT, meta.clearPickupBlueprint);
                stage.SetProperty(VanillaStageProps.END_NOTE_ID, meta.endNote);

                stage.SetProperty(VanillaStageProps.START_CAMERA_POSITION, (int)meta.startCameraPosition);
                stage.SetProperty(VanillaStageProps.START_TRANSITION, meta.startTransition);

                stage.SetProperty(EngineStageProps.TOTAL_FLAGS, meta.totalFlags);
                stage.SetProperty(EngineStageProps.FIRST_WAVE_TIME, meta.firstWaveTime);

                foreach (var pair in meta.properties)
                {
                    stage.SetProperty(pair.Key, pair.Value);
                }
            }
        }
        private void LoadEntityProperties()
        {
            foreach (EntityMeta meta in Global.Game.GetModEntityMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var entity = GetDefinition<EntityDefinition>(new NamespaceID(spaceName, meta.id));
                if (entity == null)
                    continue;
                foreach (var pair in meta.properties)
                {
                    entity.SetProperty(pair.Key, pair.Value);
                }
            }
        }
        private void AddStage(StageDefinition definition)
        {
            AddDefinition(definition);
        }
        private void PostEntityTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            if (armorResult != null && !armorResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
            {
                var entity = armorResult.Entity;
                var shellDefinition = armorResult.ShellDefinition;
                entity.PlayHitSound(armorResult.Effects, shellDefinition);
            }
            if (bodyResult != null && !bodyResult.Effects.HasEffect(VanillaDamageEffects.MUTE))
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
                passed = entity.Position.z >= targetZ - 0.03f;
            }
            // Warp downwards.
            else
            {
                passed = entity.Position.z <= targetZ + 0.03f;
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
                if (Mathf.Abs(entity.Position.z - targetZ) <= 0.05f)
                {
                    var pos = entity.Position;
                    pos.z = targetZ;
                    entity.Position = pos;
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
                if (level.Difficulty == VanillaDifficulties.easy)
                {
                    gemType = GemEffect.GemType.Emerald;
                }
                var gemEffect = GemEffect.SpawnGemEffect(level, gemType, entity.Position, entity, true);
                gemEffect.PlaySound(VanillaSoundID.points, 1 + (level.GetMaxLaneCount() - entity.GetLane() - 1) * 0.1f);
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
                    level.ChangeStage(VanillaStageID.tutorial);
                    break;
                case "prepare_starshard_tutorial":
                    {
                        var grid = level.GetGrid(4, 2);
                        if (grid == null)
                            break;
                        if (!grid.CanPlace(VanillaContraptionID.dispenser))
                            break;
                        var x = level.GetEntityColumnX(4);
                        var z = level.GetEntityLaneZ(2);
                        var y = level.GetGroundY(x, z);
                        var position = new Vector3(x, y, z);
                        level.Spawn(VanillaContraptionID.dispenser, position, null);
                    }
                    break;
                case "start_starshard_tutorial":
                    level.ChangeStage(VanillaStageID.starshard_tutorial);
                    break;
                case "start_trigger_tutorial":
                    level.ChangeStage(VanillaStageID.trigger_tutorial);
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
            if (level.StageID != VanillaStageID.halloween7)
            {
                Debug.LogError("尝试在非万圣夜场景创建第七卡槽对话框。");
                return;
            }

            var title = game.GetText(VanillaStrings.UI_PURCHASE);
            var desc = game.GetText(VanillaStrings.UI_CONFIRM_BUY_7TH_SLOT);
            var options = new string[]
            {
                game.GetText(VanillaStrings.UI_YES),
                game.GetText(VanillaStrings.UI_NO)
            };
            level.ShowDialog(title, desc, options, (index) =>
            {
                switch (index)
                {
                    case 0:
                        game.AddMoney(-750);
                        level.SetSeedSlotCount(7);
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
            if (level.StageID != VanillaStageID.prologue)
            {
                Debug.LogError("尝试在非教程场景创建教程对话框。");
                return;
            }

            var title = game.GetText(VanillaStrings.UI_TUTORIAL);
            var desc = game.GetText(VanillaStrings.UI_CONFIRM_TUTORIAL);
            var options = new string[]
            {
                game.GetText(VanillaStrings.UI_YES),
                game.GetText(VanillaStrings.UI_NO)
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
