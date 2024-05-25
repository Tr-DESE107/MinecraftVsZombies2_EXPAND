using System;
using System.Reflection;
using MVZ2.GameContent;
using MVZ2.GameContent.Seeds;
using PVZEngine;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
            AddDefinition(stageDefinitions, StageID.prologue.name, new StageDefinition(spaceName, StageID.prologue.name));

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
                    }
                }
            }
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
        public const string spaceName = "mvz2";
    }
}
