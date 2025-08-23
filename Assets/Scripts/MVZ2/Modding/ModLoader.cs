using System;
using System.Linq;
using System.Reflection;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Spawns;
using MVZ2.GameContent.Stages;
using MVZ2.Managers;
using MVZ2.Metas;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using MVZ2Logic.SeedPacks;
using MVZ2Logic.Spawns;
using PVZEngine;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Level;
using UnityEngine;

namespace MVZ2.Modding
{
    public class ModLoader
    {
        public ModLoader(MainManager main)
        {
            this.main = main;
        }
        public void Load(Mod mod, Assembly[] assemblies)
        {
            // 映射所有的属性值。
            MapProperties(mod, assemblies);

            LoadDefinitions(mod, assemblies);

            LoadDefinitionProperties(mod);
        }

        #region 初始化
        private void MapProperties(Mod mod, Assembly[] assemblies)
        {
            var nsp = mod.Namespace;
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                PropertyMapper.InitPropertyMaps(nsp, types);
            }
        }
        #endregion

        #region 加载Definitions
        private void LoadDefinitions(Mod mod, Assembly[] assemblies)
        {
            // 加载所有代码生成定义。
            LoadDefinitionsFromAssemblies(mod, assemblies);

            // 以下这这些没有相关联的定义类型，必须要手动创建。
            // 加载所有实体。
            LoadEntityMetas(mod);
            // 加载所有护甲。
            LoadArmorMetas(mod);
            // 加载所有敌人生成信息。
            LoadSpawnMetas(mod);
            // 加载所有关卡Meta。
            LoadStages(mod);
            LoadOptionBlueprints(mod);
            LoadCustomEntityBlueprints(mod);
        }
        private void LoadDefinitionsFromAssemblies(Mod mod, Assembly[] assemblies)
        {
            var nsp = mod.Namespace;
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                foreach (var type in types)
                {
                    var definitionAttr = type.GetCustomAttribute<DefinitionAttribute>();
                    if (definitionAttr == null || type.IsAbstract)
                        continue;

                    var name = definitionAttr.Name;
                    var constructor = type.GetConstructor(new Type[] { typeof(string), typeof(string) });
                    var definitionObj = constructor?.Invoke(new object[] { nsp, name });
                    if (definitionObj is Definition def)
                    {
                        mod.AddDefinition(def);
                    }
                }
            }
        }
        private void LoadEntityMetas(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (EntityMeta meta in res.GetModEntityMetas(nsp))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var def = new MetaEntityDefinition(meta.Type, nsp, name);
                var entityID = def.GetID();

                // 加载实体的属性。
                foreach (var pair in meta.Properties)
                {
                    def.SetPropertyObject(PropertyMapper.ConvertFromName(pair.Key, PropertyRegions.entity, main.BuiltinNamespace), pair.Value);
                }
                foreach (var behaviourID in meta.Behaviours)
                {
                    def.AddBehaviourID(behaviourID);
                }
                mod.AddDefinition(def);

                // 将实体作为蓝图添加到游戏中。
                var info = new EntitySeedInfo()
                {
                    entityID = entityID,
                    cost = def.GetCost(),
                    rechargeID = def.GetRechargeID(),
                    triggerActive = def.IsTriggerActive(),
                    canInstantTrigger = def.CanInstantTrigger(),
                    upgrade = def.IsUpgradeBlueprint(),
                    canInstantEvoke = def.CanInstantEvoke(),
                    model = def.GetModelID()
                };
                var blueprintID = VanillaBlueprintID.FromEntity(entityID);
                var seedDef = new EntitySeed(blueprintID.SpaceName, blueprintID.Path, info);
                mod.AddDefinition(seedDef);
            }
        }
        private void LoadArmorMetas(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (IArmorMeta meta in res.GetModArmorMetas(nsp))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var def = new MetaArmorDefinition(nsp, name, meta.ColliderConstructors);

                // 加载护甲的属性。
                foreach (var pair in meta.Properties)
                {
                    def.SetPropertyObject(PropertyMapper.ConvertFromName(pair.Key, PropertyRegions.armor, main.BuiltinNamespace), pair.Value);
                }
                foreach (var behaviourID in meta.Behaviours)
                {
                    def.AddBehaviourID(behaviourID);
                }
                mod.AddDefinition(def);
            }
        }
        private void LoadSpawnMetas(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (ISpawnMeta meta in res.GetModSpawnMetas(nsp))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var type = meta.Type;
                var spawnLevel = meta.SpawnLevel;
                var terrain = meta.Terrain;
                var weight = meta.Weight;
                var excludedTags = terrain?.ExcludedAreaTags ?? Array.Empty<NamespaceID>();
                var water = meta.Terrain?.Water ?? false;
                var air = meta.Terrain?.Air ?? false;
                var noEndless = meta.NoEndless;
                var previewEntity = meta.PreviewEntity;
                var previewVariant = meta.PreviewVariant;
                var entityID = meta.Entity;
                var entityVariant = meta.EntityVariant;
                var notInEndless = spawnLevel <= 0 || noEndless;

                VanillaSpawnDefinition spawnDef = new VanillaSpawnDefinition(nsp, name);
                if (type == "entity")
                {
                    var preview = new SpawnPreviewBehaviour(previewEntity, previewVariant);
                    var inLevel = new SpawnInLevelBehaviour(spawnDef, spawnLevel, entityID, water, air);
                    var endless = new SpawnEndlessBehaviour(notInEndless, excludedTags);
                    spawnDef.SetBehaviours(preview, inLevel, endless);
                }
                else if (name == VanillaSpawnNames.undeadFlyingObject)
                {
                    var preview = new SpawnPreviewBehaviour(previewEntity, previewVariant);
                    var inLevel = new UFOSpawnInLevelBehaviour(spawnLevel, entityVariant);
                    var endless = new SpawnEndlessBehaviour(notInEndless, excludedTags);
                    spawnDef.SetBehaviours(preview, inLevel, endless);
                }
                else if (name == VanillaSpawnNames.undeadFlyingObjectBlitz)
                {
                    var preview = new SpawnPreviewBehaviour(previewEntity, previewVariant);
                    var inLevel = new UFOSpawnInLevelBehaviour(spawnLevel, entityVariant, 0, 1, 20);
                    var endless = new SpawnEndlessBehaviour(notInEndless, excludedTags);
                    spawnDef.SetBehaviours(preview, inLevel, endless);
                }
                if (spawnDef == null)
                {
                    Debug.LogWarning($"Could not create SpawnDefinition for spawn meta {nsp}:{name}");
                    continue;
                }
                spawnDef.SetProperty(VanillaSpawnProps.MIN_SPAWN_WAVE, meta.MinSpawnWave);
                spawnDef.SetProperty(VanillaSpawnProps.PREVIEW_COUNT, meta.PreviewCount);
                if (weight != null)
                {
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_BASE, weight.Base);
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_DECAY_START, weight.DecreaseStart);
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_DECAY_END, weight.DecreaseEnd);
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_DECAY, weight.DecreasePerFlag);
                }
                mod.AddDefinition(spawnDef);
            }
        }
        private void LoadStages(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (var meta in res.GetModStageMetas(nsp))
            {
                if (meta == null)
                    continue;
                StageDefinition stageDef = null;
                switch (meta.Type)
                {
                    case StageTypes.TYPE_NORMAL:
                        {
                            stageDef = new ClassicStage(nsp, meta.ID);
                        }
                        break;
                    case StageTypes.TYPE_ENDLESS:
                        {
                            stageDef = new EndlessStage(nsp, meta.ID);
                        }
                        break;
                    default:
                        switch (meta.ID)
                        {
                            case VanillaStageNames.halloween6:
                                stageDef = new WhackAGhostStage(nsp, meta.ID);
                                break;
                            case VanillaStageNames.dream6:
                                stageDef = new BreakoutStage(nsp, meta.ID);
                                break;
                            case VanillaStageNames.castle6:
                                stageDef = new LittleZombieStage(nsp, meta.ID);
                                break;
                            case VanillaStageNames.castle7:
                                stageDef = new SeijaStage(nsp, meta.ID);
                                break;
                            case VanillaStageNames.ship6:
                                stageDef = new UFOBlitzStage(nsp, meta.ID);
                                break;

                            case VanillaStageNames.whackAGhost:
                                stageDef = new WhackAGhostStage(nsp, meta.ID);
                                break;
                            case VanillaStageNames.breakout:
                                stageDef = new BreakoutStage(nsp, meta.ID);
                                break;
                            case VanillaStageNames.bigTroubleAndLittleZombie:
                                stageDef = new LittleZombieStage(nsp, meta.ID);
                                break;
                        }
                        break;
                }
                if (stageDef != null)
                {
                    mod.AddDefinition(stageDef);
                }
            }
        }
        private void LoadOptionBlueprints(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (var option in mod.GetDefinitions<SeedOptionDefinition>(LogicDefinitionTypes.SEED_OPTION))
            {
                var seedDef = new OptionSeed(nsp, option.Name, option.GetCost());
                seedDef.SetIcon(option.GetIcon());
                seedDef.SetModelID(option.GetModelID());
                mod.AddDefinition(seedDef);
            }
        }
        private void LoadCustomEntityBlueprints(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (IEntitySeedMeta meta in res.GetModEntityBlueprintMetas(nsp))
            {
                if (meta == null)
                    continue;

                // 将实体作为蓝图添加到游戏中。
                var info = new EntitySeedInfo()
                {
                    entityID = meta.GetEntityID(),
                    cost = meta.GetCost(),
                    rechargeID = meta.GetRechargeID(),
                    triggerActive = meta.IsTriggerActive(),
                    canInstantTrigger = meta.CanInstantTrigger(),
                    upgrade = meta.IsUpgradeBlueprint(),
                    canInstantEvoke = meta.CanInstantEvoke(),
                    variant = meta.GetVariant(),
                    icon = meta.GetIcon(),
                    model = meta.GetModelID()
                };
                var seedDef = new EntitySeed(nsp, meta.ID, info);
                mod.AddDefinition(seedDef);
            }
        }
        #endregion

        #region 加载Definition属性
        public void LoadDefinitionProperties(Mod mod)
        {
            // 加载所有关卡信息。先加载全，再加载关卡属性。
            LoadStageProperties(mod);

            // 以下会通过LoadDefinitionsFromAssemblies自动创建，因此只需要读取额外信息。
            // 加载所有地形信息。
            LoadAreaProperties(mod);
            // 加载所有制品信息。
            LoadArtifactProperties(mod);
            // 加载选项蓝图信息。
            LoadSeedOptionProperties(mod);
        }
        private void LoadAreaProperties(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (IAreaMeta meta in res.GetModAreaMetas(nsp))
            {
                if (meta == null)
                    continue;
                var area = mod.GetAreaDefinition(new NamespaceID(nsp, meta.ID));
                if (area == null)
                    continue;
                area.SetProperty(VanillaAreaProps.MODEL_ID, meta.ModelID);
                area.SetProperty(VanillaLevelProps.MUSIC_ID, meta.MusicID);
                area.SetProperty(EngineAreaProps.CART_REFERENCE, meta.Cart);
                area.SetProperty(EngineAreaProps.AREA_TAGS, meta.Tags);
                area.SetProperty(VanillaAreaProps.STARSHARD_ICON, meta.StarshardIcon);

                area.SetProperty(EngineAreaProps.ENEMY_SPAWN_X, meta.EnemySpawnX);
                area.SetProperty(VanillaAreaProps.DOOR_Z, meta.DoorZ);

                area.SetProperty(VanillaAreaProps.BACKGROUND_LIGHT, meta.BackgroundLight);
                area.SetProperty(VanillaAreaProps.GLOBAL_LIGHT, meta.GlobalLight);

                area.SetProperty(EngineAreaProps.GRID_WIDTH, meta.GridWidth);
                area.SetProperty(EngineAreaProps.GRID_HEIGHT, meta.GridHeight);
                area.SetProperty(EngineAreaProps.GRID_LEFT_X, meta.GridLeftX);
                area.SetProperty(EngineAreaProps.GRID_BOTTOM_Z, meta.GridBottomZ);
                area.SetProperty(EngineAreaProps.ENTITY_LANE_Z_OFFSET, meta.EntityLaneZOffset);
                area.SetProperty(EngineAreaProps.MAX_LANE_COUNT, meta.Lanes);
                area.SetProperty(EngineAreaProps.MAX_COLUMN_COUNT, meta.Columns);

                area.SetGridLayout(meta.Grids.Select(m => m.ID).ToArray());
            }
        }
        private void LoadStageProperties(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (var meta in res.GetModStageMetas(nsp))
            {
                if (meta == null)
                    continue;
                var stage = mod.GetStageDefinition(new NamespaceID(nsp, meta.ID));
                if (stage == null)
                    continue;
                stage.SetLevelName(meta.Name);
                stage.SetDayNumber(meta.DayNumber);
                stage.SetStartEnergy(meta.StartEnergy);

                stage.SetProperty(VanillaLevelProps.MUSIC_ID, meta.MusicID);

                stage.SetProperty(VanillaStageProps.NO_START_TALK_MUSIC, meta.NoStartTalkMusic);
                stage.SetProperty<IStageTalkMeta[]>(VanillaStageProps.TALKS, meta.Talks);

                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_MODEL, meta.ClearPickupModel);
                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_CONTENT_ID, meta.ClearPickupContentID);
                stage.SetProperty(VanillaStageProps.DROPS_TROPHY, meta.DropsTrophy);
                stage.SetProperty(VanillaStageProps.END_NOTE_ID, meta.EndNote);

                stage.SetProperty(VanillaStageProps.START_CAMERA_POSITION, (int)meta.StartCameraPosition);
                stage.SetProperty(VanillaStageProps.START_TRANSITION, meta.StartTransition);

                stage.SetProperty(EngineStageProps.TOTAL_FLAGS, meta.TotalFlags);
                stage.SetProperty(EngineStageProps.FIRST_WAVE_TIME, meta.FirstWaveTime);
                stage.SetProperty(EngineStageProps.CONTINUED_FIRST_WAVE_TIME, meta.EndlessFirstWaveTime);
                stage.SetProperty(VanillaStageProps.WAVE_MAX_TIME, meta.MaxWaveTime);
                stage.SetProperty(VanillaStageProps.WAVE_ADVANCE_TIME, meta.AdvanceWaveTime);
                stage.SetProperty(VanillaStageProps.WAVE_ADVANCE_HEALTH_PERCENT, meta.AdvanceHealthPercent);

                stage.SetProperty(VanillaLevelProps.ENEMY_POOL, meta.Spawns);
                stage.SetProperty<IConveyorPoolEntry[]>(VanillaLevelProps.CONVEYOR_POOL, meta.ConveyorPool);

                stage.SetNeedBlueprints(meta.NeedBlueprints);
                stage.SetSpawnPointPower(meta.SpawnPointsPower);
                stage.SetSpawnPointMultiplier(meta.SpawnPointsMultiplier);
                stage.SetSpawnPointAddition(meta.SpawnPointsAddition);

                foreach (var pair in meta.Properties)
                {
                    stage.SetPropertyObject(PropertyMapper.ConvertFromName(pair.Key, PropertyRegions.level, Global.BuiltinNamespace), pair.Value);
                }
            }
        }
        private void LoadSeedOptionProperties(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (var meta in res.GetModBlueprintOptionMetas(nsp))
            {
                if (meta == null)
                    continue;
                var seedOptionDefinition = mod.GetSeedOptionDefinition(new NamespaceID(nsp, meta.ID));
                if (seedOptionDefinition == null)
                    continue;
                seedOptionDefinition.SetProperty(LogicSeedOptionProps.COST, meta.Cost);
                seedOptionDefinition.SetProperty(LogicSeedOptionProps.ICON, meta.GetIcon());
                seedOptionDefinition.SetProperty(LogicSeedOptionProps.MODEL_ID, meta.GetModelID());
            }
        }
        private void LoadArtifactProperties(Mod mod)
        {
            var nsp = mod.Namespace;
            foreach (IArtifactMeta meta in res.GetModArtifactMetas(nsp))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var artifact = mod.GetArtifactDefinition(new NamespaceID(nsp, name));
                if (artifact == null)
                    continue;
                artifact.SetSpriteReference(meta.Sprite);
            }
        }
        #endregion

        private ResourceManager res => main.ResourceManager;
        private MainManager main;
    }
}
