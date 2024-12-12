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
            var repeatUntilString = node.GetAttribute("repeatUntil");
            var repeatUntil = new List<NamespaceID>();
            if (!string.IsNullOrEmpty(repeatUntilString))
            {
                foreach (var str in repeatUntilString.Split(";"))
                {
                    if (NamespaceID.TryParse(str, defaultNsp, out var condition))
                    {
                        repeatUntil.Add(condition);
                    }
                }
            }
            return new StageMetaTalk()
            {
                Type = type,
                Value = value,
                RepeatUntil = repeatUntil.ToArray()
            };
        }
        public const string TYPE_START = "start";
        public const string TYPE_END = "end";
        public const string TYPE_MAP = "map";
    }
}
