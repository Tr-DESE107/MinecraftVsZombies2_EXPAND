using System.Collections.Generic;
using System.Xml;

namespace MVZ2.Metas
{
    public class CreditMetaList
    {
        public CreditsCategoryMeta[] categories;
        public static CreditMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            List<CreditsCategoryMeta> categories = new List<CreditsCategoryMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var child = node.ChildNodes[i];
                if (child.Name == "category")
                {
                    categories.Add(CreditsCategoryMeta.FromXmlNode(child, defaultNsp));
                }
            }
            return new CreditMetaList()
            {
                categories = categories.ToArray()
            };
        }
    }
}
