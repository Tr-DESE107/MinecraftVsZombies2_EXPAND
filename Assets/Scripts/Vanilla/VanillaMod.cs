using System;
using System.Linq;
using System.Reflection;
using MVZ2.GameContent;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Stages;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
            AddClassicStage(StageNames.prologue, 1,
                new EnemySpawnEntry(EnemyID.zombie),
                new EnemySpawnEntry(EnemyID.leatherCappedZombie),
                new EnemySpawnEntry(EnemyID.ironHelmettedZombie)
            );

            LoadFromAssemblies(new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) });

            Callbacks.PostEntityTakeDamage.Add(PostEntityTakeDamage);
            Callbacks.PostEntityUpdate.Add(ChangeLaneUpdate);
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
            AddDefinition(stageDefinitions, name, new ClassicStage(Namespace, name, totalFlags, enemySpawnEntries));
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
        public const string spaceName = "mvz2";
    }
}
