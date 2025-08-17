using System.Xml;

namespace MVZ2.Metas
{
    public class CommandMetaList
    {
        public CommandMeta[] metas;
        public static CommandMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new CommandMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                var meta = CommandMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
                resources[i] = meta;
            }
            return new CommandMetaList()
            {
                metas = resources,
            };
        }
    }
}
