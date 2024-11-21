using System.Xml;

namespace MVZ2Logic.Notes
{
    public class NoteMetaList
    {
        public NoteMeta[] metas;
        public static NoteMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var resources = new NoteMeta[node.ChildNodes.Count];
            for (int i = 0; i < resources.Length; i++)
            {
                resources[i] = NoteMeta.FromXmlNode(node.ChildNodes[i], defaultNsp);
            }
            return new NoteMetaList()
            {
                metas = resources,
            };
        }
    }
}
