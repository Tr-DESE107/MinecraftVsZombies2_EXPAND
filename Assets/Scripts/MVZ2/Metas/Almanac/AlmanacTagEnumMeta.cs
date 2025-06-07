﻿using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class AlmanacTagEnumMeta
    {
        public string id;
        public string type;
        public AlmanacTagEnumValueMeta[] values;
        public AlmanacTagEnumValueMeta FindValueByString(string valueString, string defaultNsp)
        {
            switch (type)
            {
                case "int":
                    if (ParseHelper.TryParseInt(valueString, out var intValue))
                    {
                        return values.FirstOrDefault(e => intValue.Equals(e.value));
                    }
                    break;
                case "id":
                    if (NamespaceID.TryParse(valueString, defaultNsp, out var idValue))
                    {
                        return values.FirstOrDefault(e => idValue.Equals(e.value));
                    }
                    break;
            }
            return null;
        }
        public static AlmanacTagEnumMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var type = node.GetAttribute("type");

            var values = new List<AlmanacTagEnumValueMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                var value = AlmanacTagEnumValueMeta.FromXmlNode(childNode, type, defaultNsp);
                values.Add(value);
            }

            return new AlmanacTagEnumMeta()
            {
                id = id,
                type = type,
                values = values.ToArray(),
            };
        }
    }
}
