using System;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using MVZ2.Level.Components;
using MVZ2.Vanilla;
using Newtonsoft.Json;
using NUnit.Framework;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Game;
using PVZEngine.Level;
using PVZEngine.Serialization;
using UnityEngine;

namespace MVZ2.Tests
{
    using LevelEngine = PVZEngine.Level.LevelEngine;
    public class SerializationTests
    {
        [Test]
        public static void EntityReferenceTest()
        {
            SerializeHelper.init();
            var game = CreateGame();
            var level = CreateLevel(game);
            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            var converters = new JsonConverter[] { };
            EntityID id = VanillaContraption.GetFragment(entity);
            var json = id.ToBson();
            var seriId = SerializeHelper.FromBson<EntityID>(json);
            var json2 = seriId.ToBson();
            Assert.AreEqual(json, json2);
        }
        [Test]
        public static void EntitySerializationTest()
        {
            SerializeHelper.init();
            var game = CreateGame();
            var level = CreateLevel(game);
            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            SerializableEntity seriEnt = entity.Serialize();
            var json = seriEnt.ToBson();

            SerializableEntity seriEnt2 = SerializeHelper.FromBson<SerializableEntity>(json);
            var entity2 = Entity.Deserialize(seriEnt2, level);

            SerializableEntity seriEnt3 = entity2.Serialize();
            var json2 = seriEnt3.ToBson();
            Assert.AreEqual(json, json2);
        }
        [Test]
        public static void LevelSerializationTest()
        {
            SerializeHelper.init();
            var game = CreateGame();
            var level = CreateLevel(game);

            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            SerializableLevel seriLevel = level.Serialize();
            var json = seriLevel.ToBson();

            SerializableLevel seriLevel2 = SerializeHelper.FromBson<SerializableLevel>(json);
            var level2 = DeserializeLevel(seriLevel2, game);

            SerializableLevel seriLevel3 = level2.Serialize();
            var json2 = seriLevel3.ToBson();

            Debug.Log(json);
            Debug.Log(json2);
            Assert.AreEqual(json, json2);
        }
        [Test]
        public static void LevelSerializationTestPrologue()
        {
            SerializeHelper.init();
            var game = CreateGame();
            var level = CreateLevel(game);

            level.Start();

            // 生成器械
            var dispenser = game.GetEntityDefinition<Dispenser>();
            var furnace = game.GetEntityDefinition<Furnace>();
            var obsidian = game.GetEntityDefinition<Obsidian>();
            var mineTNT = game.GetEntityDefinition<MineTNT>();
            for (int lane = 0; lane < level.GetMaxLaneCount(); lane++)
            {
                for (int column = 0; column < 7; column++)
                {
                    EntityDefinition def;
                    if (lane == 0 || lane == 3 || lane == 4)
                    {
                        def = dispenser;
                    }
                    else if (lane > 0 && lane < 3)
                    {
                        def = furnace;
                    }
                    else if (lane == 5)
                    {
                        def = obsidian;
                    }
                    else
                    {
                        def = mineTNT;
                    }
                    var x = level.GetEntityColumnX(column);
                    var z = level.GetEntityLaneZ(lane);
                    var height = level.GetGroundY(x, z);
                    var position = new Vector3(x, height, z);
                    level.Spawn(def, position, null);
                }
            }
            // 生成僵尸
            var zombie = EnemyID.zombie;
            var leatherCap = EnemyID.leatherCappedZombie;
            var ironHelmet = EnemyID.ironHelmettedZombie;
            for (int t = 0; t < 3; t++)
            {
                NamespaceID def = zombie;
                switch (t)
                {
                    case 1:
                        def = leatherCap;
                        break;
                    case 2:
                        def = ironHelmet;
                        break;
                }
                for (int i = 0; i < 10; i++)
                {
                    var spawnDef = game.GetSpawnDefinition(def);
                    level.SpawnEnemyAtRandomLane(spawnDef);
                }
            }

            var serializableBeforeSave = level.Serialize();
            var jsonBeforeSave = serializableBeforeSave.ToBson();

            var serializableAfterLoad = SerializeHelper.FromBson<SerializableLevel>(jsonBeforeSave);
            var jsonAfterLoad = serializableAfterLoad.ToBson();

            Debug.Log(jsonBeforeSave);
            Debug.Log(jsonAfterLoad);
            Assert.AreEqual(jsonBeforeSave, jsonAfterLoad);

            // 更新五秒
            for (int i = 0; i < 5 * level.TPS; i++)
            {
                level.Update();
            }
            var serializableAfterUpdate1 = level.Serialize();
            var jsonAfterUpdate1 = SerializeHelper.ToBson(serializableAfterUpdate1);

            var levelAfterLoad = DeserializeLevel(serializableAfterLoad, game);
            // 更新五秒
            for (int i = 0; i < 5 * level.TPS; i++)
            {
                levelAfterLoad.Update();
            }
            var serializableAfterUpdate2 = level.Serialize();
            var jsonAfterUpdate2 = SerializeHelper.ToBson(serializableAfterUpdate2);

            Debug.Log(jsonAfterUpdate1);
            Debug.Log(jsonAfterUpdate2);
            Assert.AreEqual(jsonAfterUpdate1, jsonAfterUpdate2);
        }

        private static Game CreateGame()
        {
            var game = new Game();
            var mod = new VanillaMod(game);
            game.AddMod(mod);
            return game;
        }
        private static LevelEngine CreateLevel(Game game)
        {
            var level = new LevelEngine(game, game);
            level.AddComponent(new TestHeldItemComponent(level));
            level.AddComponent(new DummyComponent(level));
            game.SetLevel(level);
            level.Init(AreaID.day, StageID.prologue, new LevelOption()
            {
                CardSlotCount = 10,
                LeftFaction = 0,
                RightFaction = 1,
                MaxEnergy = 9990,
                StartEnergy = 50,
                TPS = 30,
                StarshardSlotCount = 3
            });
            return level;
        }
        private static LevelEngine DeserializeLevel(SerializableLevel seri, Game game)
        {
            var level = LevelEngine.Deserialize(seri, game, game);
            level.AddComponent(new TestHeldItemComponent(level));
            level.AddComponent(new DummyComponent(level));
            level.DeserializeComponents(seri);
            game.SetLevel(level);
            return level;
        }
        private class TestHeldItemComponent : LevelComponent, IHeldItemComponent
        {
            public TestHeldItemComponent(LevelEngine level) : base(level, new NamespaceID("mvz2", "held_item"))
            {
            }
            public void SetHeldItem(NamespaceID type, long id, int priority, bool noCancel = false)
            {
                if (Level.IsHoldingItem() && heldItemPriority > priority)
                    return;
                heldItemType = type;
                heldItemID = id;
                heldItemPriority = priority;
                heldItemNoCancel = noCancel;
            }
            public void ResetHeldItem()
            {
                SetHeldItem(HeldTypes.none, 0, 0, false);
            }
            public bool CancelHeldItem()
            {
                if (!Level.IsHoldingItem() || HeldItemNoCancel)
                    return false;
                ResetHeldItem();
                return true;
            }

            public override ISerializableLevelComponent ToSerializable()
            {
                return new EmptySerializableLevelComponent();
            }

            public override void LoadSerializable(ISerializableLevelComponent seri)
            {
            }
            public NamespaceID HeldItemType => heldItemType;
            public long HeldItemID => heldItemID;
            public int HeldItemPriority => heldItemPriority;
            public bool HeldItemNoCancel => heldItemNoCancel;
            private NamespaceID heldItemType;
            private long heldItemID;
            private int heldItemPriority;
            private bool heldItemNoCancel;
        }
        private class DummyComponent : LevelComponent, IAdviceComponent, ILogicComponent, IMusicComponent, ISoundComponent, ITalkComponent, IUIComponent
        {
            public DummyComponent(LevelEngine level) : base(level, new NamespaceID("mvz2", "dummy"))
            {
            }

            public void ShowAdvice(string context, string textKey, int priority, int timeout)
            {
            }

            public void HideAdvice()
            {
            }

            public void Play(NamespaceID id)
            {
            }

            public void Stop()
            {
            }

            public void BeginLevel(string transition)
            {
            }

            public void StopLevel()
            {
            }

            public void PlaySound(NamespaceID id, Vector3 position, float pitch = 1)
            {
            }

            public void PlaySound(NamespaceID id, float pitch = 1)
            {
            }

            public void StartTalk(NamespaceID id, int section, float delay = 1)
            {
            }

            public void ShakeScreen(float startAmplitude, float endAmplitude, int time)
            {
            }

            public void ShowDialog(string title, string desc, string[] options, Action<int> onSelect)
            {
            }

            public void ShowMoney()
            {
            }

            public void SetHintArrowPointToBlueprint(int index)
            {
            }

            public void SetHintArrowPointToPickaxe()
            {
            }

            public void SetHintArrowPointToEntity(Entity entity)
            {
            }

            public void HideHintArrow()
            {
            }

            public override ISerializableLevelComponent ToSerializable()
            {
                return new EmptySerializableLevelComponent();
            }

            public override void LoadSerializable(ISerializableLevelComponent seri)
            {
            }
        }
    }
}
