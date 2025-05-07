using System.Text.RegularExpressions;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic.Spawns;
using PVZEngine;

namespace MVZ2.Metas
{
    public class GridLayerMeta : IGridLayerMeta
    {
        public string ID { get; private set; }
        public int Group { get; private set; }
        public int Priority { get; private set; }
        public static GridLayerMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var group = node.GetAttributeInt("group") ?? 0;
            var priority = node.GetAttributeInt("priority") ?? 0;
            return new GridLayerMeta()
            {
                ID = id,
                Group = group,
                Priority = priority,
            };
        }
    }
    public class GridErrorMeta : IGridErrorMeta
    {
        public string ID { get; private set; }
        public string Message { get; private set; }
        public static GridErrorMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var message = node.GetAttribute("message");
            return new GridErrorMeta()
            {
                ID = id,
                Message = message
            };
        }
    }
}
