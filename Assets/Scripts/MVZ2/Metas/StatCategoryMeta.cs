using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.GameContent.Recharges;
using MVZ2.IO;
using MVZ2Logic;
using MVZ2Logic.Entities;
using PVZEngine;
using PVZEngine.Entities;

namespace MVZ2.Metas
{
    public class StatCategoryMeta
    {
        public string ID { get; private set; }
        public string Name { get; private set; }
        public StatCategoryType Type { get; private set; }
        public static StatCategoryMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var name = node.GetAttribute("name");
            var typeStr = node.GetAttribute("type");
            StatCategoryType type = StatCategoryType.Entity;
            if (!string.IsNullOrEmpty(typeStr) && typeDict.TryGetValue(typeStr, out var value))
            {
                type = value;
            }
            return new StatCategoryMeta()
            {
                ID = id,
                Name = name,
                Type = type
            };
        }
        private static readonly Dictionary<string, StatCategoryType> typeDict = new Dictionary<string, StatCategoryType>()
        {
            { "entity", StatCategoryType.Entity }
        };
    }
    public enum StatCategoryType
    {
        Entity
    }
}
