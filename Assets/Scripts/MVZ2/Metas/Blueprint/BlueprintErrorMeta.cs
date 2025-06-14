using System.Xml;
using MVZ2.IO;
using MVZ2Logic.SeedPacks;

namespace MVZ2.Metas
{
    public class BlueprintErrorMeta : IBlueprintErrorMeta
    {
        public string ID { get; private set; }
        public string Message { get; private set; }
        public static BlueprintErrorMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var message = node.GetAttribute("message");
            return new BlueprintErrorMeta()
            {
                ID = id,
                Message = message,
            };
        }
    }
}
