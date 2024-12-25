using System.Xml;
using MVZ2.IO;
using MVZ2Logic;

namespace MVZ2.Metas
{
    public class ChapterTransitionMeta
    {
        public string ID { get; private set; }
        public float Angle { get; private set; }
        public bool NoRotate { get; private set; }
        public SpriteReference TextSprite { get; private set; }
        public static ChapterTransitionMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var angle = node.GetAttributeFloat("angle") ?? 0;
            var noRotate = node.GetAttributeBool("noRotate") ?? false;
            var textSprite = node.GetAttributeSpriteReference("textSprite", defaultNsp);
            return new ChapterTransitionMeta()
            {
                ID = id,
                Angle = angle,
                NoRotate = noRotate,
                TextSprite = textSprite
            };
        }
    }
}
