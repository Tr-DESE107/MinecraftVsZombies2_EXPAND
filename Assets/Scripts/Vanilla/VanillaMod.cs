using System;
using System.Collections.Generic;
using System.Reflection;
using MVZ2.GameContent;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Projectiles;
using MVZ2.GameContent.Seeds;
using MVZ2.Vanilla.Buffs;
using PVZEngine;
using UnityEditor;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
            AddDefinition(stageDefinitions, StageID.prologue.name, new StageDefinition());

            LoadFromAssemblies(new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) });

            Callbacks.PostEntityTakeDamage.Add(PostEntityTakeDamage);
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
                        var definition = Activator.CreateInstance(type);
                        var name = definitionAttr.Name;
                        AddDefinitionByObject(definition, name);
                        var seedEntityAttr = type.GetCustomAttribute<EntitySeedDefinitionAttribute>();
                        if (seedEntityAttr != null)
                        {
                            var seedDef = new EntitySeed(
                                new NamespaceID(Namespace, name),
                                seedEntityAttr.Cost,
                                seedEntityAttr.RechargeID,
                                seedEntityAttr.TriggerActive,
                                seedEntityAttr.TriggerCost);
                            AddDefinition(seedDefinitions, name, seedDef);
                        }
                    }
                }
            }
        }

        private void PostEntityTakeDamage(DamageResult bodyResult, DamageResult armorResult)
        {
            if (armorResult != null && !armorResult.Effects.HasEffect(DamageEffects.MUTE))
            {
                var entity = armorResult.Entity;
                var shellId = entity.EquipedArmor.GetShellID();
                var shellDefinition = entity.Game.GetShellDefinition(shellId);
                entity.PlayHitSound(armorResult.Effects, shellDefinition);
            }
            if (bodyResult != null && !bodyResult.Effects.HasEffect(DamageEffects.MUTE))
            {
                var entity = bodyResult.Entity;
                var shellId = entity.GetShellID();
                var shellDefinition = entity.Game.GetShellDefinition(shellId);
                entity.PlayHitSound(bodyResult.Effects, shellDefinition);
            }
        }
        public const string spaceName = "mvz2";
    }
}
