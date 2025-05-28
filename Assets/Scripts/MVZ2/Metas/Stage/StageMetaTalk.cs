﻿using System.Xml;
using MVZ2.IO;
using MVZ2.Saves;
using MVZ2Logic.Games;
using MVZ2Logic.Level;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StageMetaTalk : IStageTalkMeta
    {
        public string Type { get; private set; }
        public NamespaceID Value { get; private set; }
        public int StartSection { get; private set; }
        public XMLConditionList RepeatCondition { get; private set; }
        public static StageMetaTalk FromXmlNode(XmlNode node, string defaultNsp)
        {
            var type = node.GetAttribute("type");
            var value = node.GetAttributeNamespaceID("value", defaultNsp);
            var startSection = node.GetAttributeInt("section") ?? 0;

            XMLConditionList repeatCondition = null;
            var conditionNode = node["repeat"];
            if (conditionNode != null)
            {
                repeatCondition = XMLConditionList.FromXmlNode(conditionNode, defaultNsp);
            }
            return new StageMetaTalk()
            {
                Type = type,
                Value = value,
                StartSection = startSection,
                RepeatCondition = repeatCondition
            };
        }
        public bool ShouldRepeat(IGameSaveData save)
        {
            if (RepeatCondition == null)
                return false;
            return save.MeetsXMLConditions(RepeatCondition);
        }
        public const string TYPE_START = "start";
        public const string TYPE_END = "end";
        public const string TYPE_MAP = "map";
    }
}
