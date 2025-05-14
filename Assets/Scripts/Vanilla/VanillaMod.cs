using System;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using MVZ2.GameContent.Implements;
using MVZ2.GameContent.Seeds;
using MVZ2.GameContent.Shells;
using MVZ2.GameContent.Spawns;
using MVZ2.GameContent.Stages;
using MVZ2.Vanilla.Contraptions;
using MVZ2.Vanilla.Entities;
using MVZ2.Vanilla.Level;
using MVZ2.Vanilla.Saves;
using MVZ2.Vanilla.SeedPacks;
using MVZ2Logic;
using MVZ2Logic.Artifacts;
using MVZ2Logic.Entities;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using MVZ2Logic.Modding;
using MVZ2Logic.Saves;
using MVZ2Logic.SeedPacks;
using MVZ2Logic.Spawns;
using PVZEngine;
using PVZEngine.Armors;
using PVZEngine.Base;
using PVZEngine.Definitions;
using PVZEngine.Entities;
using PVZEngine.Level;
using PVZEngine.Level.Collisions;
using UnityEngine;

namespace MVZ2.Vanilla
{
    public class VanillaMod : Mod
    {
        public VanillaMod() : base(spaceName)
        {
        }
        public override void Init(IGame game, Assembly[] assemblies)
        {
            base.Init(game, assemblies);
            // 映射所有的属性值。
            MapProperties(assemblies);
            // 加载所有代码生成定义。
            LoadDefinitionsFromAssemblies(assemblies);

            // 以下这这些没有相关联的定义类型，必须要手动创建。
            // 加载所有实体。
            LoadEntityMetas(game);
            // 加载所有护甲。
            LoadArmorMetas(game);
            // 加载所有敌人生成信息。
            LoadSpawnMetas(game);

            // 加载所有关卡信息。先加载全，再加载关卡属性。
            LoadStages(game);
            LoadStageProperties(game);

            // 以下会通过LoadDefinitionsFromAssemblies自动创建，因此只需要读取额外信息。
            // 加载所有地形信息。
            LoadAreaProperties(game);
            // 加载所有制品信息。
            LoadArtifactProperties(game);
            // 加载选项蓝图信息。
            LoadSeedOptionProperties(game);

            // 回调。
            ImplementCallbacks(new GemStageImplements());
            ImplementCallbacks(new BlueprintImplements());
            ImplementCallbacks(new StatsImplements());
            ImplementCallbacks(new EntityImplements());
            ImplementCallbacks(new DifficultyImplements());
            ImplementCallbacks(new CartToMoneyImplements());
            ImplementCallbacks(new TalkActionImplements());
            ImplementCallbacks(new BlueprintRecommendImplements());
            ImplementCallbacks(new WaterImplements());
            ImplementCallbacks(new AchievementsImplements());
            ImplementCallbacks(new RandomChinaImplements());
        }
        public override void LateInit(IGame game)
        {
            base.LateInit(game);
            // 为护甲添加对应的护甲行为。
            LoadArmorBehaviours(game);
            // 为实体添加对应的实体行为。
            LoadEntityBehaviours(game);
            // 加载所有实体蓝图。
            LoadEntityBlueprints(game);
            // 加载所有选项蓝图。
            LoadOptionBlueprints(game);
        }
        public override void PostGameInit()
        {
            base.PostGameInit();
            SerializeHelper.RegisterClass<SerializableVanillaSaveData>();
        }

        #region 存档
        public override ModSaveData CreateSaveData()
        {
            return new VanillaSaveData(spaceName);
        }
        public override ModSaveData LoadSaveData(string json)
        {
            var serializable = SerializeHelper.FromBson<SerializableVanillaSaveData>(json);
            return serializable.Deserialize();
        }
        #endregion

        #region 回调
        private void ImplementCallbacks(VanillaImplements implements)
        {
            implements.Implement(this);
        }
        #endregion

        protected void MapProperties(Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
            {
                var types = assembly.GetTypes();
                PropertyMapper.InitPropertyMaps(Namespace, types);
            }
        }
        protected void LoadDefinitionsFromAssemblies(Assembly[] assemblies)
        {
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
                    var definitionObj = constructor?.Invoke(new object[] { Namespace, name });
                    if (definitionObj is Definition def)
                    {
                        AddDefinition(def);
                    }
                }
            }
        }

        #region 加载定义
        private void LoadEntityMetas(IGame game)
        {
            foreach (IEntityMeta meta in game.GetModEntityMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var def = new MetaEntityDefinition(meta.Type, spaceName, name);

                // 加载实体的属性。
                foreach (var pair in meta.Properties)
                {
                    def.SetProperty(PropertyMapper.ConvertFromName(pair.Key, PropertyRegions.entity, Global.BuiltinNamespace), pair.Value);
                }
                AddDefinition(def);
            }
        }
        private void LoadArmorMetas(IGame game)
        {
            foreach (IArmorMeta meta in game.GetModArmorMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var def = new MetaArmorDefinition(spaceName, name, meta.ColliderConstructors);

                // 加载护甲的属性。
                foreach (var pair in meta.Properties)
                {
                    def.SetProperty(PropertyMapper.ConvertFromName(pair.Key, PropertyRegions.armor, Global.BuiltinNamespace), pair.Value);
                }
                AddDefinition(def);
            }
        }
        private void LoadSpawnMetas(IGame game)
        {
            foreach (ISpawnMeta meta in game.GetModSpawnMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var spawnLevel = meta.SpawnLevel;
                var terrain = meta.Terrain;
                var weight = meta.Weight;
                var excludedTags = terrain?.ExcludedAreaTags ?? Array.Empty<NamespaceID>();
                var water = meta.Terrain?.Water ?? false;

                var spawnDef = new VanillaSpawnDefinition(Namespace, name, spawnLevel, new NamespaceID(Namespace, name), excludedTags);
                spawnDef.SetProperty(VanillaSpawnProps.MIN_SPAWN_WAVE, meta.MinSpawnWave);
                spawnDef.SetProperty(VanillaSpawnProps.PREVIEW_COUNT, meta.PreviewCount);
                if (weight != null)
                {
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_BASE, weight.Base);
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_DECAY_START, weight.DecreaseStart);
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_DECAY_END, weight.DecreaseEnd);
                    spawnDef.SetProperty(VanillaSpawnProps.WEIGHT_DECAY, weight.DecreasePerFlag);
                }
                spawnDef.CanSpawnAtWaterLane = water;
                AddDefinition(spawnDef);
            }
        }
        private void LoadStages(IGame game)
        {
            AddDefinition(new WhackAGhostStage(spaceName, VanillaStageNames.halloween6));
            AddDefinition(new BreakoutStage(spaceName, VanillaStageNames.dream6));
            AddDefinition(new LittleZombieStage(spaceName, VanillaStageNames.castle6));
            AddDefinition(new SeijaStage(spaceName, VanillaStageNames.castle7));
            AddDefinition(new IZombieStage(spaceName, VanillaStageNames.mausoleum6));
            foreach (var meta in game.GetModStageMetas(spaceName))
            {
                if (meta == null)
                    continue;
                switch (meta.Type)
                {
                    case StageTypes.TYPE_NORMAL:
                        {
                            var stage = new ClassicStage(spaceName, meta.ID);
                            AddDefinition(stage);
                        }
                        break;
                    case StageTypes.TYPE_ENDLESS:
                        {
                            var stage = new EndlessStage(spaceName, meta.ID);
                            AddDefinition(stage);
                        }
                        break;
                }
            }
        }
        #endregion

        #region 加载属性
        private void LoadAreaProperties(IGame game)
        {
            foreach (IAreaMeta meta in game.GetModAreaMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var area = this.GetAreaDefinition(new NamespaceID(spaceName, meta.ID));
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
        private void LoadStageProperties(IGame game)
        {
            foreach (var meta in game.GetModStageMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var stage = this.GetStageDefinition(new NamespaceID(spaceName, meta.ID));
                if (stage == null)
                    continue;
                stage.SetLevelName(meta.Name);
                stage.SetDayNumber(meta.DayNumber);
                stage.SetStartEnergy(meta.StartEnergy);

                stage.SetProperty(VanillaLevelProps.MUSIC_ID, meta.MusicID);

                stage.SetProperty(VanillaStageProps.NO_START_TALK_MUSIC, meta.NoStartTalkMusic);
                stage.SetProperty(VanillaStageProps.TALKS, meta.Talks);

                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_MODEL, meta.ClearPickupModel);
                stage.SetProperty(VanillaStageProps.CLEAR_PICKUP_BLUEPRINT, meta.ClearPickupBlueprint);
                stage.SetProperty(VanillaStageProps.END_NOTE_ID, meta.EndNote);

                stage.SetProperty(VanillaStageProps.START_CAMERA_POSITION, (int)meta.StartCameraPosition);
                stage.SetProperty(VanillaStageProps.START_TRANSITION, meta.StartTransition);

                stage.SetProperty(EngineStageProps.TOTAL_FLAGS, meta.TotalFlags);
                stage.SetProperty(EngineStageProps.FIRST_WAVE_TIME, meta.FirstWaveTime);

                stage.SetProperty(VanillaLevelProps.ENEMY_POOL, meta.Spawns);
                stage.SetProperty(VanillaLevelProps.CONVEYOR_POOL, meta.ConveyorPool);

                stage.SetNeedBlueprints(meta.NeedBlueprints);
                stage.SetSpawnPointPower(meta.SpawnPointsPower);
                stage.SetSpawnPointMultiplier(meta.SpawnPointsMultiplier);
                stage.SetSpawnPointAddition(meta.SpawnPointsAddition);

                foreach (var pair in meta.Properties)
                {
                    stage.SetProperty(PropertyMapper.ConvertFromName(pair.Key, PropertyRegions.level, Global.BuiltinNamespace), pair.Value);
                }
            }
        }
        private void LoadSeedOptionProperties(IGame game)
        {
            foreach (var meta in game.GetModSeedOptionMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var seedOptionDefinition = this.GetSeedOptionDefinition(new NamespaceID(spaceName, meta.ID));
                if (seedOptionDefinition == null)
                    continue;
                seedOptionDefinition.SetProperty(LogicSeedOptionProps.COST, meta.Cost);
                seedOptionDefinition.SetProperty(LogicSeedOptionProps.ICON, meta.Icon);
            }
        }
        private void LoadArtifactProperties(IGame game)
        {
            foreach (IArtifactMeta meta in game.GetModArtifactMetas(spaceName))
            {
                if (meta == null)
                    continue;
                var name = meta.ID;
                var artifact = this.GetArtifactDefinition(new NamespaceID(spaceName, name));
                if (artifact == null)
                    continue;
                artifact.SetUnlockID(meta.Unlock);
                artifact.SetSpriteReference(meta.Sprite);
            }
        }
        #endregion

        #region 后处理
        private void LoadArmorBehaviours(IGame game)
        {
            foreach (ArmorDefinition def in GetDefinitions<ArmorDefinition>(EngineDefinitionTypes.ARMOR))
            {
                if (def == null)
                    continue;
                var meta = game.GetArmorMeta(def.GetID());
                if (meta == null)
                    continue;
                foreach (var behaviourID in meta.Behaviours)
                {
                    def.AddBehaviour(behaviourID);
                }
            }
        }
        private void LoadEntityBehaviours(IGame game)
        {
            foreach (var entityDef in GetDefinitions<EntityDefinition>(EngineDefinitionTypes.ENTITY))
            {
                if (entityDef == null)
                    continue;
                var entityID = entityDef.GetID();
                var meta = game.GetEntityMeta(entityID);
                if (meta != null)
                {
                    foreach (var behaviourID in meta.Behaviours)
                    {
                        entityDef.AddBehaviourID(behaviourID);
                    }
                }
                var mainBehaviour = this.GetEntityBehaviourDefinition(entityID);
                if (mainBehaviour != null)
                {
                    entityDef.AddBehaviourID(entityID);
                }
            }
        }
        private void LoadEntityBlueprints(IGame game)
        {
            foreach (EntityDefinition def in GetDefinitions<EntityDefinition>(EngineDefinitionTypes.ENTITY))
            {
                if (def == null)
                    continue;
                var id = def.GetID();

                // 将实体作为蓝图添加到游戏中。
                var info = new EntitySeedInfo()
                {
                    entityID = id,
                    cost = def.GetCost(),
                    rechargeID = def.GetRechargeID(),
                    triggerActive = def.IsTriggerActive(),
                    canInstantTrigger = def.CanInstantTrigger(),
                    upgrade = def.IsUpgradeBlueprint(),
                    canInstantEvoke = def.CanInstantEvoke()
                };
                var blueprintID = VanillaBlueprintID.FromEntity(id);
                var seedDef = new EntitySeed(blueprintID.SpaceName, blueprintID.Path, info);
                AddDefinition(seedDef);
            }
        }
        private void LoadOptionBlueprints(IGame game)
        {
            foreach (var option in GetDefinitions<SeedOptionDefinition>(LogicDefinitionTypes.SEED_OPTION))
            {
                var seedDef = new OptionSeed(Namespace, option.Name, option.GetCost());
                AddDefinition(seedDef);
            }
        }
        #endregion

        public const string spaceName = "mvz2";
    }
}
