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
            LoadStages();
            LoadFromAssemblies(new Assembly[] { Assembly.GetAssembly(typeof(VanillaMod)) });
            LoadEntityMetas();

            ImplementCallbacks(new GemStageImplements());
            ImplementCallbacks(new StarshardSpawnImplements());
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
                                seedEntityAttr.RechargeID);
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
        private void LoadEntityMetas()
        {
            foreach (IEntityMeta meta in Global.Game.GetModEntityMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var entity = GetDefinition<EntityDefinition>(new NamespaceID(spaceName, meta.ID));
                if (entity == null)
                    continue;
                foreach (var pair in meta.Properties)
                {
                    entity.SetProperty(pair.Key, pair.Value);
                }
            }
        }
        public const string spaceName = "mvz2";
    }
}
