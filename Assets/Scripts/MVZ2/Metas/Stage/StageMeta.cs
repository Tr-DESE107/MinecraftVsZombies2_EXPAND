using System;
using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Level;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StageMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int DayNumber { get; private set; }
        public string Type { get; private set; }
        [Obsolete]
        public NamespaceID[] Unlocks { get; private set; }
        public XMLConditionList UnlockConditions { get; private set; }
        public float StartEnergy { get; private set; }

        public NamespaceID MusicID { get; private set; }

        public bool NoStartTalkMusic { get; private set; }
        public StageMetaTalk[] Talks { get; private set; }

        public string ModelPreset { get; private set; }

        public NamespaceID ClearPickupModel { get; private set; }
        public NamespaceID ClearPickupContentID { get; private set; }
        public bool DropsTrophy { get; private set; }
        public NamespaceID EndNote { get; private set; }

        public LevelCameraPosition StartCameraPosition { get; private set; }
        public string StartTransition { get; private set; }

        public int TotalFlags { get; private set; }
        public float SpawnPointsPower { get; private set; }
        public float SpawnPointsMultiplier { get; private set; }
        public float SpawnPointsAddition { get; private set; }
        public NamespaceID[] Spawns { get; private set; }
        public ConveyorPoolEntry[] ConveyorPool { get; private set; }

        public float FirstWaveTime { get; private set; }
        public float EndlessFirstWaveTime { get; private set; }
        public float MaxWaveTime { get; private set; }
        public float AdvanceWaveTime { get; private set; }
        public float AdvanceHealthPercent { get; private set; }

        public bool NeedBlueprints { get; private set; }

        public Dictionary<string, object> Properties { get; private set; }
        public static StageMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type") ?? StageTypes.TYPE_NORMAL;
            var dayNumber = node.GetAttributeInt("dayNumber") ?? 0;
            var startEnergy = node.GetAttributeFloat("startEnergy") ?? 50;
            var musicID = node.GetAttributeNamespaceID("music", defaultNsp);
            var needBlueprints = node.GetAttributeBool("needBlueprints") ?? true;

            var unlockNode = node["unlock"];
            XMLConditionList unlockConditions = null;
            if (unlockNode != null)
            {
                unlockConditions = XMLConditionList.FromXmlNode(unlockNode, defaultNsp);
            }
            else
            {
                var unlocks = node.GetAttributeNamespaceIDArray("unlock", defaultNsp);
                var condition = new XMLCondition()
                {
                    Required = unlocks
                };
                unlockConditions = new XMLConditionList(condition);
            }

            var modelNode = node["model"];
            var preset = modelNode?.GetAttribute("preset") ?? "default";

            var talks = new List<StageMetaTalk>();
            var talksNode = node["talks"];
            var noStartTalkMusic = false;
            if (talksNode != null)
            {
                noStartTalkMusic = talksNode.GetAttributeBool("noStartMusic") ?? false;
                for (int i = 0; i < talksNode.ChildNodes.Count; i++)
                {
                    var child = talksNode.ChildNodes[i];
                    if (child.Name == "talk")
                    {
                        talks.Add(StageMetaTalk.FromXmlNode(child, defaultNsp));
                    }
                }
            }

            var clearNode = node["clear"];
            var clearPickupModel = clearNode?.GetAttributeNamespaceID("pickupModel", defaultNsp);
            var clearPickupContentID = clearNode?.GetAttributeNamespaceID("pickupContentID", defaultNsp) ?? clearNode?.GetAttributeNamespaceID("blueprint", defaultNsp);
            var dropsTrophy = clearNode?.GetAttributeBool("trophy") ?? false;
            var endNote = clearNode?.GetAttributeNamespaceID("note", defaultNsp);

            var cameraNode = node["camera"];
            var cameraPositionStr = cameraNode?.GetAttribute("position");
            var startCameraPosition = cameraPositionDict.TryGetValue(cameraPositionStr ?? string.Empty, out var p) ? p : LevelCameraPosition.House;
            var transition = cameraNode?.GetAttribute("transition");

            var conveyorNode = node["conveyor"];
            var conveyorPool = new ConveyorPoolEntry[conveyorNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < conveyorPool.Length; i++)
            {
                conveyorPool[i] = ConveyorPoolEntry.FromXmlNode(conveyorNode.ChildNodes[i], defaultNsp);
            }

            var spawnNode = node["spawns"];
            var flags = spawnNode?.GetAttributeInt("flags") ?? 1;
            var spawnPointsPower = spawnNode?.GetAttributeFloat("pointsPower") ?? 1;
            var spawnPointsMultiplier = spawnNode?.GetAttributeFloat("pointsMultiplier") ?? 1;
            var spawnPointsAddition = spawnNode?.GetAttributeFloat("pointsAddition") ?? 0;
            var spawns = new NamespaceID[spawnNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < spawns.Length; i++)
            {
                var childNode = spawnNode.ChildNodes[i];
                spawns[i] = childNode.GetAttributeNamespaceID("id", defaultNsp);
            }

            var timeNode = node["time"];
            var firstWaveTime = timeNode?.GetAttributeFloat("firstWave") ?? Ticks.ToSeconds(spawnNode?.GetAttributeInt("firstWaveTime") ?? 540);
            var endlessFirstWaveTime = timeNode?.GetAttributeFloat("endlessFirstWave") ?? 6f;
            var maxWaveTime = timeNode?.GetAttributeFloat("waveMax") ?? 30;
            var advanceWaveTime = timeNode?.GetAttributeFloat("waveAdvance") ?? 10;
            var advanceHealthPercent = timeNode?.GetAttributeFloat("waveAdvanceHealthPercent") ?? 0.6f;

            var propertiesNode = node["properties"];
            var properties = propertiesNode.ToPropertyDictionary(defaultNsp);
            return new StageMeta()
            {
                ID = id,
                Name = name,
                DayNumber = dayNumber,
                Type = type,
                StartEnergy = startEnergy,
                UnlockConditions = unlockConditions,
                MusicID = musicID,

                ModelPreset = preset,

                NoStartTalkMusic = noStartTalkMusic,
                Talks = talks.ToArray(),

                ClearPickupModel = clearPickupModel,
                ClearPickupContentID = clearPickupContentID,
                DropsTrophy = dropsTrophy,
                EndNote = endNote,

                StartCameraPosition = startCameraPosition,
                StartTransition = transition,

                ConveyorPool = conveyorPool,

                TotalFlags = flags,
                Spawns = spawns,

                FirstWaveTime = firstWaveTime,
                EndlessFirstWaveTime = endlessFirstWaveTime,
                MaxWaveTime = maxWaveTime,
                AdvanceWaveTime = advanceWaveTime,
                AdvanceHealthPercent = advanceHealthPercent,

                SpawnPointsPower = spawnPointsPower,
                SpawnPointsMultiplier = spawnPointsMultiplier,
                SpawnPointsAddition = spawnPointsAddition,

                NeedBlueprints = needBlueprints,

                Properties = properties
            };
        }
        public static readonly Dictionary<string, LevelCameraPosition> cameraPositionDict = new Dictionary<string, LevelCameraPosition>()
        {
            { "house", LevelCameraPosition.House },
            { "lawn", LevelCameraPosition.Lawn },
            { "choose", LevelCameraPosition.Choose },
        };
    }
    public class ConveyorPoolEntry : IConveyorPoolEntry
    {
        public NamespaceID ID { get; }
        public int Count { get; }
        public int MinCount { get; }
        public ConveyorPoolEntry(NamespaceID id, int count = 1, int minCount = 1)
        {
            ID = id;
            Count = count;
            MinCount = minCount;
        }
        public static ConveyorPoolEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var count = node.GetAttributeInt("count") ?? 1;
            var minCount = node.GetAttributeInt("minCount") ?? 1;
            return new ConveyorPoolEntry(id, count, minCount);
        }
    }
}
