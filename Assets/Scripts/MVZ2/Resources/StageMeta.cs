using System.Collections.Generic;
using System.Xml;
using MVZ2.GameContent;
using MVZ2.Level;
using PVZEngine;
using PVZEngine.Definitions;
using PVZEngine.Level;

namespace MVZ2.Resources
{
    public class StageMeta
    {
        public string id;
        public string name;
        public string type;
        public int totalFlags;
        public NamespaceID startTalk;
        public NamespaceID endTalk;
        public NamespaceID unlock;
        public NamespaceID clearPickupModel;
        public NamespaceID clearPickupBlueprint;
        public NamespaceID endNote;

        public LevelCameraPosition startCameraPosition;
        public string startTransition;

        public EnemySpawnEntry[] spawns;
        public static StageMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type") ?? "normal";
            var totalFlags = node.GetAttributeInt("totalFlags") ?? 1;
            var startTalk = node.GetAttributeNamespaceID("startTalk", defaultNsp);
            var endTalk = node.GetAttributeNamespaceID("endTalk", defaultNsp);
            var endNote = node.GetAttributeNamespaceID("endNote", defaultNsp);
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var clearPickupModel = node.GetAttributeNamespaceID("clearPickupModel", defaultNsp);
            var clearPickupBlueprint = node.GetAttributeNamespaceID("clearPickupBlueprint", defaultNsp);

            var cameraNode = node["camera"];
            var cameraPositionStr = cameraNode?.GetAttribute("position");
            var startCameraPosition = cameraPositionDict.TryGetValue(cameraPositionStr ?? string.Empty, out var p) ? p : LevelCameraPosition.House;
            var transition = cameraNode?.GetAttribute("transition");

            var spawns = new EnemySpawnEntry[node.ChildNodes.Count];
            for (int i = 0; i < spawns.Length; i++)
            {
                spawns[i] = EnemySpawnEntry.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new StageMeta()
            {
                id = id,
                name = name,
                type = type,
                totalFlags = totalFlags,
                startTalk = startTalk,
                endTalk = endTalk,
                endNote = endNote,
                unlock = unlock,

                startCameraPosition = startCameraPosition,
                startTransition = transition,
                spawns = spawns,
                clearPickupModel = clearPickupModel,
                clearPickupBlueprint = clearPickupBlueprint
            };
        }
        public const string TYPE_NORMAL = "normal";
        public const string TYPE_SPECIAL = "special";
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
