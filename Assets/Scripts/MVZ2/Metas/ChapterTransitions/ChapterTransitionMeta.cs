using System.Xml;
using MVZ2.IO;
using MVZ2Logic;

namespace MVZ2.Metas
{
    public class ChapterTransitionMeta
    {
        public string ID { get; private set; }
        public float Angle { get; private set; }
        public int Mode { get; private set; }
        public SpriteReference TextSprite { get; private set; }
        public static ChapterTransitionMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var id = node.GetAttribute("id");
            var angle = node.GetAttributeFloat("angle") ?? 0;
            var mode = node.GetAttributeInt("noRotate") ?? 0;
            var textSprite = node.GetAttributeSpriteReference("textSprite", defaultNsp);
            return new ChapterTransitionMeta()
            {
                ID = id,
                Angle = angle,
                Mode = mode,
                TextSprite = textSprite
            };
        }
        public const int MODE_ROTATE = 0;
        public const int MODE_NO_ROTATE = 1;
        public const int MODE_END = 2;
    }
}
