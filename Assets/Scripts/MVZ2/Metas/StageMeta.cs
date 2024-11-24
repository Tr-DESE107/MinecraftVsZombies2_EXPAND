using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Level;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StageMeta : IStageMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public int DayNumber { get; private set; }
        public string Type { get; private set; }
        public NamespaceID Unlock { get; private set; }

        public NamespaceID StartTalk { get; private set; }
        public NamespaceID EndTalk { get; private set; }
        public NamespaceID MapTalk { get; private set; }

        public NamespaceID ClearPickupModel { get; private set; }
        public NamespaceID ClearPickupBlueprint { get; private set; }
        public NamespaceID EndNote { get; private set; }

        public LevelCameraPosition StartCameraPosition { get; private set; }
        public string StartTransition { get; private set; }

        public int TotalFlags { get; private set; }
        public EnemySpawnEntry[] Spawns { get; private set; }
        public int FirstWaveTime { get; private set; }

        public Dictionary<string, object> Properties { get; private set; }

        IEnemySpawnEntry[] IStageMeta.Spawns => Spawns;
        public static StageMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type") ?? StageTypes.TYPE_NORMAL;
            var dayNumber = node.GetAttributeInt("dayNumber") ?? 0;
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);

            var talkNode = node["talk"];
            var startTalk = talkNode?.GetAttributeNamespaceID("start", defaultNsp);
            var endTalk = talkNode?.GetAttributeNamespaceID("end", defaultNsp);
            var mapTalk = talkNode?.GetAttributeNamespaceID("map", defaultNsp);

            var clearNode = node["clear"];
            var clearPickupModel = clearNode?.GetAttributeNamespaceID("pickupModel", defaultNsp);
            var clearPickupBlueprint = clearNode?.GetAttributeNamespaceID("blueprint", defaultNsp);
            var endNote = clearNode?.GetAttributeNamespaceID("note", defaultNsp);

            var cameraNode = node["camera"];
            var cameraPositionStr = cameraNode?.GetAttribute("position");
            var startCameraPosition = cameraPositionDict.TryGetValue(cameraPositionStr ?? string.Empty, out var p) ? p : LevelCameraPosition.House;
            var transition = cameraNode?.GetAttribute("transition");

            var spawnNode = node["spawns"];
            var flags = spawnNode?.GetAttributeInt("flags") ?? 1;
            var firstWaveTime = spawnNode?.GetAttributeInt("firstWaveTime") ?? 540;
            var spawns = new EnemySpawnEntry[spawnNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < spawns.Length; i++)
            {
                spawns[i] = EnemySpawnEntry.FromXmlNode(spawnNode.ChildNodes[i], defaultNsp);
            }

            var propertiesNode = node["properties"];
            var properties = propertiesNode.ToPropertyDictionary(defaultNsp);
            return new StageMeta()
            {
                ID = id,
                Name = name,
                DayNumber = dayNumber,
                Type = type,
                Unlock = unlock,

                StartTalk = startTalk,
                EndTalk = endTalk,
                MapTalk = mapTalk,

                ClearPickupModel = clearPickupModel,
                ClearPickupBlueprint = clearPickupBlueprint,
                EndNote = endNote,

                StartCameraPosition = startCameraPosition,
                StartTransition = transition,

                TotalFlags = flags,
                FirstWaveTime = firstWaveTime,
                Spawns = spawns,

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
    public class EnemySpawnEntry : IEnemySpawnEntry
    {
        public NamespaceID SpawnRef { get; }
        public int EarliestFlag { get; }
        public EnemySpawnEntry(NamespaceID spawnRef, int earliestFlag = 0)
        {
            SpawnRef = spawnRef;
            EarliestFlag = earliestFlag;
        }
        public static EnemySpawnEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var spawnRef = node.GetAttributeNamespaceID("id", defaultNsp);
            var earliestFlag = node.GetAttributeInt("earliestFlag") ?? 0;
            return new EnemySpawnEntry(spawnRef, earliestFlag);
        }
    }
}
