using System;
using System.Collections;
using System.Threading.Tasks;
using MVZ2.GameContent.Areas;
using MVZ2.GameContent.Contraptions;
using MVZ2.GameContent.Enemies;
using MVZ2.GameContent.Stages;
using MVZ2.Level;
using MVZ2.Managers;
using MVZ2.Vanilla.Entities;
using MVZ2Logic;
using MVZ2Logic.Games;
using NUnit.Framework;
using PVZEngine;
using PVZEngine.Entities;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace MVZ2.Tests
{
    using LevelEngine = PVZEngine.Level.LevelEngine;
    public class SerializationTests
    {
        [UnityTest]
        public static IEnumerator EntityReferenceTest()
        {
            var areaId = VanillaAreaID.day;
            var stageId = VanillaStageID.prologue;
            yield return Init();
            yield return GotoLevel();
            var levelController = GetLevelController();
            InitLevel(levelController, areaId, stageId);

            var game = GetGame();
            var level = GetLevel();

            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            EntityID id = entity.GetFragment();
            var json = id.ToBson();
            var seriId = SerializeHelper.FromBson<EntityID>(json);
            var json2 = seriId.ToBson();
            Assert.AreEqual(json, json2);
        }
        [UnityTest]
        public static IEnumerator EntitySerializationTest()
        {
            var areaId = VanillaAreaID.day;
            var stageId = VanillaStageID.prologue;
            yield return Init();
            yield return GotoLevel();
            var levelController = GetLevelController();
            InitLevel(levelController, areaId, stageId);

            var game = GetGame();
            var level = GetLevel();

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
        [UnityTest]
        public static IEnumerator LevelSerializationTest()
        {
            var areaId = VanillaAreaID.day;
            var stageId = VanillaStageID.prologue;
            yield return Init();
            yield return GotoLevel();
            var levelController = GetLevelController();
            InitLevel(levelController, areaId, stageId);

            var game = GetGame();
            var level = GetLevel();

            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            SerializableLevelController seriLevel = levelController.SaveGame();
            var json = seriLevel.ToBson();

            SerializableLevelController seriLevel2 = SerializeHelper.FromBson<SerializableLevelController>(json);
            yield return GotoLevel();
            var levelController2 = GetLevelController();
            LoadLevel(levelController2, seriLevel2, areaId, stageId);

            SerializableLevelController seriLevel3 = levelController2.SaveGame();
            var json2 = seriLevel3.ToBson();

            Debug.Log(json);
            Debug.Log(json2);
            Assert.AreEqual(json, json2);
        }
        [UnityTest]
        public static IEnumerator LevelSerializationTestPrologue()
        {
            var areaId = VanillaAreaID.day;
            var stageId = VanillaStageID.prologue;
            yield return Init();
            yield return GotoLevel();
            var levelController = GetLevelController();
            InitLevel(levelController, areaId, stageId);

            var game = GetGame();
            var level = GetLevel();


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
            var zombie = VanillaEnemyID.zombie;
            var leatherCap = VanillaEnemyID.leatherCappedZombie;
            var ironHelmet = VanillaEnemyID.ironHelmettedZombie;
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

            var serializableBeforeSave = levelController.SaveGame();
            var jsonBeforeSave = serializableBeforeSave.ToBson();

            var serializableAfterLoad = SerializeHelper.FromBson<SerializableLevelController>(jsonBeforeSave);
            var jsonAfterLoad = serializableAfterLoad.ToBson();

            Debug.Log(jsonBeforeSave);
            Debug.Log(jsonAfterLoad);
            Assert.AreEqual(jsonBeforeSave, jsonAfterLoad);

            // 更新五秒
            for (int i = 0; i < 5 * level.TPS; i++)
            {
                levelController.UpdateLogic();
            }
            var serializableAfterUpdate1 = levelController.SaveGame();
            var jsonAfterUpdate1 = SerializeHelper.ToBson(serializableAfterUpdate1);

            yield return GotoLevel();
            var levelAfterLoad = GetLevelController();
            LoadLevel(levelAfterLoad, serializableAfterLoad, areaId, stageId);
            // 更新五秒
            for (int i = 0; i < 5 * level.TPS; i++)
            {
                levelAfterLoad.UpdateLogic();
            }
            var serializableAfterUpdate2 = levelAfterLoad.SaveGame();
            var jsonAfterUpdate2 = SerializeHelper.ToBson(serializableAfterUpdate2);

            Debug.Log(jsonAfterUpdate1);
            Debug.Log(jsonAfterUpdate2);
            Assert.AreEqual(jsonAfterUpdate1, jsonAfterUpdate2);
        }
        private static IEnumerator Init()
        {
            return InitAsync().ToCoroutineFunc();
        }
        private static async Task InitAsync()
        {
            if (inited)
                return;
            var op = Addressables.LoadSceneAsync("TestScene", LoadSceneMode.Single);
            var scene = await op.Task;
            TestEntrance entrance = null;
            foreach (var rootGameObj in scene.Scene.GetRootGameObjects())
            {
                entrance = rootGameObj.GetComponentInChildren<TestEntrance>();
                if (entrance)
                {
                    break;
                }
            }
            if (!entrance)
                throw new InvalidOperationException("Test Entrance not found.");

            await entrance.Init();
            inited = true;
        }
        private static IGame GetGame()
        {
            return Global.Game;
        }
        private static LevelEngine GetLevel()
        {
            return Global.Game.GetLevel();
        }
        private static LevelController GetLevelController()
        {
            return Main.LevelManager.GetLevel();
        }
        private static void InitLevel(LevelController level, NamespaceID areaId, NamespaceID stageId)
        {
            level.InitLevel(Main.Game, areaId, stageId);
            level.StartGame();
        }
        private static void LoadLevel(LevelController level, SerializableLevelController seri, NamespaceID areaId, NamespaceID stageId)
        {
            level.LoadGame(seri, Main.Game, areaId, stageId);
            level.Resume();
        }
        private static IEnumerator GotoLevel()
        {
            return Global.GotoLevel();
        }
        private static MainManager Main => MainManager.Instance;
        private static bool inited = false;
    }
}
