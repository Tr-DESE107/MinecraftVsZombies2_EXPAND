using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using Newtonsoft.Json;
using NUnit.Framework;
using PVZEngine.Game;
using PVZEngine.Level;
using PVZEngine.Serialization;
using Tools;
using UnityEngine;

namespace MVZ2.Tests
{
    using LevelEngine = PVZEngine.Level.LevelEngine;
    public class SerializationTests
    {
        [Test]
        public static void EntityReferenceTest()
        {
            var game = CreateGame();
            var level = CreateLevel(game);
            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            var converters = new JsonConverter[] { };
            var json = JsonConvert.SerializeObject(VanillaContraption.GetFragment(entity), converters);
            var dese = JsonConvert.DeserializeObject<EntityReferenceChain>(json, converters);
            var json2 = JsonConvert.SerializeObject(dese, converters);
            Assert.AreEqual(json, json2);
        }
        [Test]
        public static void EntitySerializationTest()
        {
            var game = CreateGame();
            var level = CreateLevel(game);
            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            var converters = new JsonConverter[] { new Vector3Converter(), new Vector2Converter(), new ColorConverter() };
            var json = JsonConvert.SerializeObject(entity.Serialize(), converters);
            var dese = JsonConvert.DeserializeObject<SerializableEntity>(json, converters);
            var entity2 = Entity.Deserialize(dese, level);
            var json2 = JsonConvert.SerializeObject(entity2.Serialize(), converters);
            Assert.AreEqual(json, json2);
        }
        [Test]
        public static void LevelSerializationTest()
        {
            var game = CreateGame();
            var level = new LevelEngine(game, game);
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
            game.SetLevel(level);

            var definition = game.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            var converters = new JsonConverter[] { new Vector3Converter(), new Vector2Converter(), new ColorConverter() };
            var json = JsonConvert.SerializeObject(level.Serialize(), converters);
            var dese = JsonConvert.DeserializeObject<SerializableLevel>(json, converters);
            var level2 = LevelEngine.Deserialize(dese, game, game);
            game.SetLevel(level2);
            var json2 = JsonConvert.SerializeObject(level2.Serialize(), converters);
            Debug.Log(json);
            Debug.Log(json2);
            Assert.AreEqual(json, json2);
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
    }
}
