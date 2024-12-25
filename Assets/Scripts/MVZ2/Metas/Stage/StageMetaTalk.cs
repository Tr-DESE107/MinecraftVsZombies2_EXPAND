using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Level;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StageMetaTalk : IStageTalkMeta
    {
        public string Type { get; private set; }
        public NamespaceID Value { get; private set; }
        public NamespaceID[] RepeatUntil { get; private set; }
        public static StageMetaTalk FromXmlNode(XmlNode node, string defaultNsp)
        {
            var type = node.GetAttribute("type");
            var value = node.GetAttributeNamespaceID("value", defaultNsp);
            var repeatUntil = node.GetAttributeNamespaceIDArray("repeatUntil", defaultNsp);
            return new StageMetaTalk()
            {
                Type = type,
                Value = value,
                RepeatUntil = repeatUntil
            };
        }
        public const string TYPE_START = "start";
        public const string TYPE_END = "end";
        public const string TYPE_MAP = "map";
    }
}
