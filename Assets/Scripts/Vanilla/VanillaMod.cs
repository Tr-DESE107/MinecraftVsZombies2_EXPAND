using System;
using System.Reflection;
using MVZ2.GameContent;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Stages;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Game;
using PVZEngine.LevelManaging;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
            var classicStage = new ClassicStage(spaceName, StageNames.prologue, 1,
                new EnemySpawnEntry[]
                {
                    new EnemySpawnEntry(EnemyID.zombie),
                    new EnemySpawnEntry(EnemyID.leatherCappedZombie),
                    new EnemySpawnEntry(EnemyID.ironHelmettedZombie)
                }
            );
            classicStage.SetProperty(StageProps.START_TALK, TalkID.tutorial);
            AddStage(classicStage);

            LoadFromAssemblies(new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) });

            LevelCallbacks.PostEntityTakeDamage.Add(PostEntityTakeDamage);
            LevelCallbacks.PostEntityUpdate.Add(ChangeLaneUpdate);
            GameCallbacks.TalkAction.Add(TalkAction);
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
                        var definition = constructor?.Invoke(new object[] { Namespace, name });
                        AddDefinitionByObject(definition, name);
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
                            AddDefinition(seedDefinitions, name, seedDef);
                        }
                        var spawnDefAttr = type.GetCustomAttribute<SpawnDefinitionAttribute>();
                        if (spawnDefAttr != null)
                        {
                            var spawnDef = new SpawnDefinition(Namespace, name, spawnDefAttr.SpawnCost, new NamespaceID(Namespace, name));
                            AddDefinition(spawnDefinitions, name, spawnDef);
                        }
                    }
                }
            }
        }

        private void AddClassicStage(string name, int totalFlags, params EnemySpawnEntry[] enemySpawnEntries)
        {
            AddStage(new ClassicStage(Namespace, name, totalFlags, enemySpawnEntries));
        }
        private void AddStage(StageDefinition definition)
        {
            AddDefinition(stageDefinitions, definition.Name, definition);
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
        private void TalkAction(ITalkSystem system, string cmd, string[] parameters)
        {
            var game = Game;
            if (!game.IsInLevel())
            {
                return;
            }
            Level level = game.GetLevel();
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
            }
        }
        private void ShowSeventhSlotDialog(ITalkSystem system)
        {
            var game = Game;
            if (!game.IsInLevel())
            {
                return;
            }
            Level level = game.GetLevel();
            if (level.StageID != StageID.halloween7)
            {
                Debug.LogError("尝试在非万圣夜场景创建第七卡槽对话框。");
                return;
            }

            var title = Game.GetText(TextID.UI.purchase);
            var desc = Game.GetText(TextID.UI.confirmPurchaseSeventhSlot);
            var options = new string[]
            {
                Game.GetText(TextID.UI.yes),
                Game.GetText(TextID.UI.no)
            };
            level.ShowDialog(title, desc, options, (index) =>
            {
                switch (index)
                {
                    case 0:
                        Game.AddMoney(-750);
                        level.SetSeedPackCount(7);
                        Game.SetBlueprintSlots(7);
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
            var game = Game;
            if (!game.IsInLevel())
            {
                return;
            }
            Level level = game.GetLevel();
            if (level.StageID != StageID.prologue)
            {
                Debug.LogError("尝试在非教程场景创建教程对话框。");
                return;
            }

            var title = Game.GetText(TextID.UI.tutorial);
            var desc = Game.GetText(TextID.UI.confirmTutorial);
            var options = new string[]
            {
                Game.GetText(TextID.UI.yes),
                Game.GetText(TextID.UI.no)
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
            var game = Game;
            if (!game.IsInLevel())
            {
                return;
            }
            Level level = game.GetLevel();
            level.ShowMoney();
            if (Game.GetMoney() >= 750)
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
