using System.Xml;
using MVZ2.IO;
using PVZEngine;
using UnityEngine;

namespace MVZ2.Metas
{
    public class MapMeta
    {
        public string id;
        public Vector2 size;
        public NamespaceID area;
        public LoreTalkMetaList loreTalks;
        public MapPreset[] presets;
        public NamespaceID[] stages;
        public NamespaceID endlessStage;
        public static MapMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var area = node.GetAttributeNamespaceID("area", defaultNsp);
            var width = node.GetAttributeFloat("width") ?? 4080;
            var height = node.GetAttributeFloat("height") ?? 2400;
            var size = new Vector2(width, height);

            var presetsNode = node["presets"];
            var presets = new MapPreset[presetsNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < presets.Length; i++)
            {
                presets[i] = MapPreset.FromXmlNode(presetsNode.ChildNodes[i], defaultNsp);
            }

            var loreTalks = LoreTalkMetaList.FromXmlNode(node["talks"], defaultNsp);

            var stagesNode = node["stages"];
            var stages = new NamespaceID[stagesNode?.ChildNodes?.Count ?? 0];
            NamespaceID endlessStage = null;
            if (stagesNode != null)
            {
                endlessStage = stagesNode.GetAttributeNamespaceID("endless", defaultNsp);
                for (int i = 0; i < stages.Length; i++)
                {
                    stages[i] = stagesNode.ChildNodes[i].GetAttributeNamespaceID("id", defaultNsp);
                }
            }
            return new MapMeta()
            {
                id = id,
                size = size,
                area = area,
                loreTalks = loreTalks,
                presets = presets,
                stages = stages,
                endlessStage = endlessStage,
            };
        }
    }
    public class MapPreset
    {
        public NamespaceID id;
        public NamespaceID model;
        public NamespaceID music;
        public int priority;
        public XMLConditionList conditions;
        public Color backgroundColor;
        public static MapPreset FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            var backgroundColor = node.GetAttributeColor("backgroundColor") ?? Color.black;
            var priority = node.GetAttributeInt("priority") ?? 0;
            XMLConditionList conditions = null;
            var conditionsNode = node["conditions"];
            if (conditionsNode != null)
            {
                conditions = XMLConditionList.FromXmlNode(conditionsNode, defaultNsp);
            }
            return new MapPreset()
            {
                id = id,
                model = model,
                music = music,
                priority = priority,
                backgroundColor = backgroundColor,
                conditions = conditions,
            };
        }
    }
}
