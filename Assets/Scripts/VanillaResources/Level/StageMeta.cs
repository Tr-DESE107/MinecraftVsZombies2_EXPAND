using System.Collections.Generic;
using System.Xml;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2Logic.Level
{
    public class StageMeta
    {
        public string id;
        public string name;
        public int dayNumber;
        public string type;
        public NamespaceID unlock;

        public NamespaceID startTalk;
        public NamespaceID endTalk;
        public NamespaceID mapTalk;

        public NamespaceID clearPickupModel;
        public NamespaceID clearPickupBlueprint;
        public NamespaceID endNote;

        public LevelCameraPosition startCameraPosition;
        public string startTransition;

        public int totalFlags;
        public EnemySpawnEntry[] spawns;
        public int firstWaveTime;

        public Dictionary<string, object> properties;
        public static StageMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type") ?? "normal";
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
                id = id,
                name = name,
                dayNumber = dayNumber,
                type = type,
                unlock = unlock,

                startTalk = startTalk,
                endTalk = endTalk,
                mapTalk = mapTalk,

                clearPickupModel = clearPickupModel,
                clearPickupBlueprint = clearPickupBlueprint,
                endNote = endNote,

                startCameraPosition = startCameraPosition,
                startTransition = transition,

                totalFlags = flags,
                firstWaveTime = firstWaveTime,
                spawns = spawns,

                properties = properties
            };
        }
        public const string TYPE_NORMAL = "normal";
        public const string TYPE_SPECIAL = "special";
        public const string TYPE_MINIGAME = "minigame";
        public static readonly Dictionary<string, LevelCameraPosition> cameraPositionDict = new Dictionary<string, LevelCameraPosition>()
        {
            { "house", LevelCameraPosition.House },
            { "lawn", LevelCameraPosition.Lawn },
            { "choose", LevelCameraPosition.Choose },
        };
    }
    public class EnemySpawnEntry : IEnemySpawnEntry
    {
        public NamespaceID spawnRef;
        public int earliestFlag;
        public EnemySpawnEntry(NamespaceID spawnRef, int earliestFlag = 0)
        {
            this.spawnRef = spawnRef;
            this.earliestFlag = earliestFlag;
        }

        public bool CanSpawn(LevelEngine game)
        {
            return game.CurrentFlag >= earliestFlag;
        }

        public SpawnDefinition GetSpawnDefinition(IContentProvider game)
        {
            return game.GetSpawnDefinition(spawnRef);
        }
        public static EnemySpawnEntry FromXmlNode(XmlNode node, string defaultNsp)
        {
            var spawnRef = node.GetAttributeNamespaceID("id", defaultNsp);
            var earliestFlag = node.GetAttributeInt("earliestFlag") ?? 0;
            return new EnemySpawnEntry(spawnRef, earliestFlag);
        }
    }
}
