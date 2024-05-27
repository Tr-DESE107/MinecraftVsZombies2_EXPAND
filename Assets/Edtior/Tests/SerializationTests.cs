using System;
using System.Collections.Generic;
using MVZ2.GameContent;
using MVZ2.GameContent.Contraptions;
using MVZ2.Vanilla;
using Newtonsoft.Json;
using NUnit.Framework;
using PVZEngine;
using PVZEngine.Serialization;
using UnityEngine;

namespace MVZ2.Tests
{
    public class SerializationTests
    {
        [Test]
        public static void EntityReferenceTest()
        {
            var level = CreateLevel();
            var definition = level.GetEntityDefinition<Dispenser>();
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
            var level = CreateLevel();
            var definition = level.GetEntityDefinition<Dispenser>();
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
            var mod = new VanillaMod();
            var level = new Game(mod);
            level.Init(AreaID.day, StageID.prologue, new GameOption()
            {
                CardSlotCount = 10,
                LeftFaction = 0,
                RightFaction = 1,
                MaxEnergy = 9990,
                StartEnergy = 50,
                TPS = 30,
                StarshardSlotCount = 3
            });
            var definition = level.GetEntityDefinition<Dispenser>();
            var entity = level.Spawn(definition, new Vector3(500, 0, 300), null);

            var converters = new JsonConverter[] { new Vector3Converter(), new Vector2Converter(), new ColorConverter() };
            var json = JsonConvert.SerializeObject(level.Serialize(), converters);
            var dese = JsonConvert.DeserializeObject<SerializableLevel>(json, converters);
            var level2 = Game.Deserialize(dese, new Mod[] { mod });
            var json2 = JsonConvert.SerializeObject(level2.Serialize(), converters);
            Debug.Log(json);
            Debug.Log(json2);
            Assert.AreEqual(json, json2);
        }

        private static Game CreateLevel()
        {
            var mod = new VanillaMod();
            var level = new Game(mod);
            level.Init(AreaID.day, StageID.prologue, new GameOption()
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
