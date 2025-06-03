using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MVZ2.Metas
{
    public class AlmanacMetaList
    {
        public List<AlmanacTagMeta> tags;
        public List<AlmanacTagEnumMeta> enums;
        public List<AlmanacCategory> categories;
        public AlmanacCategory GetCategory(string name)
        {
            return categories.FirstOrDefault(c => c.name == name);
        }
        public bool TryGetCategory(string name, out AlmanacCategory category)
        {
            category = GetCategory(name);
            return category != null;
        }
        public static AlmanacMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var tags = new List<AlmanacTagMeta>();
            var enums = new List<AlmanacTagEnumMeta>();
            var categories = new List<AlmanacCategory>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                if (childNode.Name == "tags")
                {
                    LoadTags(childNode, defaultNsp, tags, enums);
                }
                else
                {
                    var category = AlmanacCategory.FromXmlNode(childNode, defaultNsp);
                    categories.Add(category);
                }
            }
            return new AlmanacMetaList()
            {
                tags = tags,
                enums = enums,
                categories = categories,
            };
        }
        private static void LoadTags(XmlNode node, string defaultNsp, List<AlmanacTagMeta> tags, List<AlmanacTagEnumMeta> enums)
        {
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                if (childNode.Name == "tag")
                {
                    var tag = AlmanacTagMeta.FromXmlNode(childNode, defaultNsp);
                    tags.Add(tag);
                }
                else if (childNode.Name == "enum")
                {
                    var enumType = AlmanacTagEnumMeta.FromXmlNode(childNode, defaultNsp);
                    enums.Add(enumType);
                }
            }
        }
    }
}
