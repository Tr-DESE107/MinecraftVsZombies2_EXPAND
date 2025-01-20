using System.Collections.Generic;
using System.Xml;
using MVZ2.IO;
using MVZ2Logic;
using PVZEngine;

namespace MVZ2.Metas
{
    public class StoreChatGroupMeta
    {
        public NamespaceID Character { get; private set; }
        public StoreChatMeta[] Chats { get; private set; }
        public static StoreChatGroupMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var character = node.GetAttributeNamespaceID("character", defaultNsp);

            List<StoreChatMeta> chats = new List<StoreChatMeta>();
            for (int i = 0; i < node.ChildNodes.Count; i++)
            {
                var childNode = node.ChildNodes[i];
                if (childNode.Name == "chat")
                {
                    chats.Add(StoreChatMeta.FromXmlNode(childNode, defaultNsp));
                }
            }
            return new StoreChatGroupMeta()
            {
                Character = character,
                Chats = chats.ToArray(),
            };
        }
    }
}
