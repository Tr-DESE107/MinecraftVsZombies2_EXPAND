using System.Xml;
using MVZ2.IO;

namespace MVZ2.Metas
{
    public class BlueprintErrorMeta
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
