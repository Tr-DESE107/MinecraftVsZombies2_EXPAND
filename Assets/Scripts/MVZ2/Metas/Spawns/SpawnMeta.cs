using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Spawns;
using PVZEngine;

namespace MVZ2.Metas
{
    public class SpawnMeta : ISpawnMeta
    {
        public string ID { get; private set; }
        public NamespaceID Entity { get; private set; }
        public int SpawnLevel { get; private set; }
        public int PreviewCount { get; private set; }
        public SpawnTerrainMeta Terrain { get; private set; }
        public SpawnWeightMeta Weight { get; private set; }
        ISpawnTerrainMeta ISpawnMeta.Terrain => Terrain;
        ISpawnWeightMeta ISpawnMeta.Weight => Weight;
        public static SpawnMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var entity = node.GetAttributeNamespaceID("id", defaultNsp);

            int level = 1;
            var spawnNode = node["spawn"];
            if (spawnNode != null)
            {
                level = spawnNode.GetAttributeInt("level") ?? 1;
            }

            int previewCount = 1;
            var previewNode = node["preview"];
            if (previewNode != null)
            {
                previewCount = previewNode.GetAttributeInt("count") ?? 1;
            }

            SpawnTerrainMeta terrain = null;
            var terrainNode = node["terrain"];
            if (terrainNode != null)
            {
                terrain = SpawnTerrainMeta.FromXmlNode(terrainNode, defaultNsp);
            }

            SpawnWeightMeta weight = null;
            var weightNode = node["weight"];
            if (weightNode != null)
            {
                weight = SpawnWeightMeta.FromXmlNode(weightNode);
            }
            return new SpawnMeta()
            {
                ID = id,
                Entity = entity,
                SpawnLevel = level,
                PreviewCount = previewCount,
                Terrain = terrain,
                Weight = weight
            };
        }
    }
    public class SpawnTerrainMeta : ISpawnTerrainMeta
    {
        public NamespaceID[] ExcludedAreaTags { get; private set; }
        public bool Water { get; private set; }
        public bool Air { get; private set; }
        public static SpawnTerrainMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var excludedAreaTags = node.GetAttributeNamespaceIDArray("excludedTags", defaultNsp);
            var water = node.GetAttributeBool("water") ?? false;
            var air = node.GetAttributeBool("air") ?? false;
            return new SpawnTerrainMeta()
            {
                ExcludedAreaTags = excludedAreaTags,
                Water = water,
                Air = air
            };
        }
    }
    public class SpawnWeightMeta : ISpawnWeightMeta
    {
        public int Base { get; private set; }
        public int DecreaseStart { get; private set; }
        public int DecreaseEnd { get; private set; }
        public int DecreasePerFlag { get; private set; }
        public static SpawnWeightMeta FromXmlNode(XmlNode node)
        {
            var baseValue = node.GetAttributeInt("base") ?? 0;
            var start = node.GetAttributeInt("decreaseStart") ?? 0;
            var end = node.GetAttributeInt("decreaseEnd") ?? 0;
            var perFlag = node.GetAttributeInt("decreasePerFlag") ?? 0;
            return new SpawnWeightMeta()
            {
                Base = baseValue,
                DecreaseStart = start,
                DecreaseEnd = end,
                DecreasePerFlag = perFlag
            };
        }
    }
}
