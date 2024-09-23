using System.Xml;
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
        public NamespaceID unlock;
        public NamespaceID clearPickupModel;
        public NamespaceID clearPickupBlueprint;
        public EnemySpawnEntry[] spawns;
        public static StageMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var type = node.GetAttribute("type") ?? "normal";
            var totalFlags = node.GetAttributeInt("totalFlags") ?? 1;
            var startTalk = node.GetAttributeNamespaceID("startTalk", defaultNsp);
            var unlock = node.GetAttributeNamespaceID("unlock", defaultNsp);
            var clearPickupModel = node.GetAttributeNamespaceID("clearPickupModel", defaultNsp);
            var clearPickupBlueprint = node.GetAttributeNamespaceID("clearPickupBlueprint", defaultNsp);

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
                unlock = unlock,
                spawns = spawns,
                clearPickupModel = clearPickupModel,
                clearPickupBlueprint = clearPickupBlueprint
            };
        }
        public const string TYPE_NORMAL = "normal";
        public const string TYPE_SPECIAL = "special";
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
