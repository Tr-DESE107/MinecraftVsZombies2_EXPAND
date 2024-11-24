using System.Xml;
using PVZEngine;
using UnityEngine;
using MVZ2.IO;

namespace MVZ2.Metas
{
    public class MapMeta
    {
        public string id;
        public NamespaceID area;
        public NamespaceID endlessUnlock;
        public MapPreset[] presets;
        public NamespaceID[] stages;
        public static MapMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var endlessUnlock = node.GetAttributeNamespaceID("endlessUnlock", defaultNsp);
            var area = node.GetAttributeNamespaceID("area", defaultNsp);

            var presetsNode = node["presets"];
            var presets = new MapPreset[presetsNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < presets.Length; i++)
            {
                presets[i] = MapPreset.FromXmlNode(presetsNode.ChildNodes[i], defaultNsp);
            }

            var stagesNode = node["stages"];
            var stages = new NamespaceID[stagesNode?.ChildNodes.Count ?? 0];
            for (int i = 0; i < stages.Length; i++)
            {
                stages[i] = stagesNode.ChildNodes[i].GetAttributeNamespaceID("id", defaultNsp);
            }
            return new MapMeta()
            {
                id = id,
                endlessUnlock = endlessUnlock,
                area = area,
                presets = presets,
                stages = stages,
            };
        }
    }
    public class MapPreset
    {
        public NamespaceID id;
        public NamespaceID model;
        public NamespaceID music;
        public Color backgroundColor;
        public static MapPreset FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttributeNamespaceID("id", defaultNsp);
            var model = node.GetAttributeNamespaceID("model", defaultNsp);
            var music = node.GetAttributeNamespaceID("music", defaultNsp);
            var backgroundColor = node.GetAttributeColor("backgroundColor") ?? Color.black;
            return new MapPreset()
            {
                id = id,
                model = model,
                music = music,
                backgroundColor = backgroundColor,
            };
        }
    }
}
