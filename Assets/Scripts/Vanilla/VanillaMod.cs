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
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Grids;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Entities;
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
            LoadEntityMetas();
            LoadArtifactMetas();
            LoadStages();
            LoadDefinitionsFromAssemblies(new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) });
            AddEntityBehaviours();

            ImplementCallbacks(new GemStageImplements());
            ImplementCallbacks(new StarshardSpawnImplements());
            ImplementCallbacks(new StatsImplements());
            ImplementCallbacks(new EntityImplements());
            ImplementCallbacks(new DifficultyImplements());
            ImplementCallbacks(new CartToMoneyImplements());
            ImplementCallbacks(new TalkActionImplements());
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
        private void ImplementCallbacks(VanillaImplements implements)
        {
            implements.Implement(this);
        }
        private void LoadEntityMetas()
        {
            foreach (IEntityMeta meta in Global.Game.GetModEntityMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var entityDefinition = new MetaEntityDefinition(meta.Type, spaceName, name);
                foreach (var pair in meta.Properties)
                {
                    entityDefinition.SetProperty(pair.Key, pair.Value);
                }
                AddDefinition(entityDefinition);

                var seedDef = new EntitySeed(Namespace, name, entityDefinition.GetCost(), entityDefinition.GetRechargeID(), entityDefinition.IsTriggerActive());
                AddDefinition(seedDef);

                var spawnCost = entityDefinition.GetSpawnCost();
                if (spawnCost > 0)
                {
                    var spawnDef = new SpawnDefinition(Namespace, name, spawnCost, new NamespaceID(Namespace, name));
                    spawnDef.SetProperty(VanillaSpawnProps.PREVIEW_COUNT, entityDefinition.GetPreviewCount());
                    AddDefinition(spawnDef);
                }
            }
        }
        private void LoadArtifactMetas()
        {
            foreach (IArtifactMeta meta in Global.Game.GetModArtifactMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var definition = new MetaArtifactDefinition(meta, spaceName, name);
                AddDefinition(definition);
            }
        }
        private void LoadStages()
        {
            AddDefinition(new DebugStage(spaceName, VanillaStageNames.debug));
            AddDefinition(new TutorialStage(spaceName, VanillaStageNames.tutorial));
            AddDefinition(new StarshardTutorialStage(spaceName, VanillaStageNames.starshardTutorial));

            foreach (var meta in Global.Game.GetModStageMetas(spaceName).Where(m => m.Type == StageTypes.TYPE_NORMAL))
            {
                if (meta == null)
                    continue;
                var stage = new ClassicStage(spaceName, meta.ID);
                stage.SetProperty(VanillaLevelProps.ENEMY_POOL, meta.Spawns);
                AddDefinition(stage);
            }
            foreach (var meta in Global.Game.GetModStageMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var stage = GetDefinition<StageDefinition>(new NamespaceID(spaceName, meta.ID));
                if (stage == null)
                    continue;
                stage.SetLevelName(meta.Name);
                stage.SetDayNumber(meta.DayNumber);

                stage.SetProperty(VanillaStageProps.START_TALK, meta.StartTalk);
                stage.SetProperty(VanillaStageProps.END_TALK, meta.EndTalk);
                stage.SetProperty(VanillaStageProps.MAP_TALK, meta.MapTalk);

                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_MODEL, meta.ClearPickupModel);
                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_BLUEPRINT, meta.ClearPickupBlueprint);
                stage.SetProperty(VanillaStageProps.END_NOTE_ID, meta.EndNote);

                stage.SetProperty(VanillaStageProps.START_CAMERA_POSITION, (int)meta.StartCameraPosition);
                stage.SetProperty(VanillaStageProps.START_TRANSITION, meta.StartTransition);

                stage.SetProperty(EngineStageProps.TOTAL_FLAGS, meta.TotalFlags);
                stage.SetProperty(EngineStageProps.FIRST_WAVE_TIME, meta.FirstWaveTime);

                foreach (var pair in meta.Properties)
                {
                    stage.SetProperty(pair.Key, pair.Value);
                }
            }
        }
        protected void LoadDefinitionsFromAssemblies(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    var definitionAttr = type.GetCustomAttribute<DefinitionAttribute>();
                    if (definitionAttr == null || type.IsAbstract)
                        continue;

                    var name = definitionAttr.Name;
                    var constructor = type.GetConstructor(new Type[] { typeof(string), typeof(string) });
                    var definitionObj = constructor?.Invoke(new object[] { Namespace, name });
                    if (definitionObj is Definition def)
                    {
                        AddDefinition(def);
                    }
                }
            }
        }
        private void AddEntityBehaviours()
        {
            foreach (var behaviour in GetDefinitions<EntityBehaviourDefinition>())
            {
                var entity = GetDefinition<EntityDefinition>(behaviour.GetMatchEntityID());
                if (entity == null)
                    continue;
                if (entity.GetBehaviour() != null)
                {
                    Debug.LogWarning($"Entity {entity.GetID()} has multiple Entity Behaviours.");
                }
                entity.SetBehaviour(behaviour);
            }
        }

        public const string spaceName = "mvz2";
    }
}
