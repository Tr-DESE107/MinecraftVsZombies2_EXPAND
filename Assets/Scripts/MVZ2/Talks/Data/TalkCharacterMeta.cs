using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2.Talk
{
    public class TalkCharacterMetaList
    {
        public List<TalkCharacterMeta> metas = new List<TalkCharacterMeta>();
        public static TalkCharacterMetaList FromXmlNode(XmlNode node, string defaultNsp)
        {
            var list = new TalkCharacterMetaList();
            var metaChildNodes = node.ChildNodes;
            for (int i = 0; i < metaChildNodes.Count; i++)
            {
                var child = metaChildNodes[i];
                list.metas.Add(TalkCharacterMeta.FromXmlNode(child, defaultNsp));
            }
            return list;
        }
    }

    public class TalkCharacterMeta
    {
        public string name;
        public NamespaceID unlockCondition;
        public List<TalkCharacterVariant> variants = new List<TalkCharacterVariant>();
        public static TalkCharacterMeta FromXmlNode(XmlNode node, string defaultNsp)
        {
            var meta = new TalkCharacterMeta();
            meta.name = node.GetAttribute("name");
            meta.unlockCondition = node.GetAttributeNamespaceID("unlock", defaultNsp);

            var variantChildNodes = node.ChildNodes;
            for (int i = 0; i < variantChildNodes.Count; i++)
            {
                var child = variantChildNodes[i];
                meta.variants.Add(TalkCharacterVariant.FromXmlNode(child, defaultNsp));
            }
            return meta;
        }

        public TalkCharacterVariant GetVariant(NamespaceID id)
        {
            return variants.FirstOrDefault(v => v.id == id);
        }
    }

    public class TalkCharacterVariant
    {
        public NamespaceID id;
        public int width;
        public int height;
        public float pivotX;
        public float pivotY;
        public NamespaceID parent;
        public List<TalkCharacterLayer> layers = new List<TalkCharacterLayer>();

        public static TalkCharacterVariant FromXmlNode(XmlNode node, string defaultNsp)
        {
            var variant = new TalkCharacterVariant();
            variant.id = node.GetAttributeNamespaceID("id", defaultNsp);
            variant.width = node.GetAttributeInt("width") ?? 0;
            variant.height = node.GetAttributeInt("height") ?? 0;
            variant.pivotX = node.GetAttributeFloat("pivotX") ?? 0.5f;
            variant.pivotY = node.GetAttributeFloat("pivotY") ?? 0.5f;
            variant.parent = node.GetAttributeNamespaceID("parent", defaultNsp);

            var variantChildNodes = node.ChildNodes;
            for (int i = 0; i < variantChildNodes.Count; i++)
            {
                var child = variantChildNodes[i];
                variant.layers.Add(TalkCharacterLayer.FromXmlNode(node, defaultNsp));
            }
            return variant;
        }
    }

    public class TalkCharacterLayer
    {
        public SpriteReference sprite;
        public int positionX;
        public int positionY;

        public static TalkCharacterLayer FromXmlNode(XmlNode node, string defaultNsp)
        {
            var layer = new TalkCharacterLayer();
            layer.sprite = node.GetAttributeSpriteReference("sprite", defaultNsp);
            layer.positionX = node.GetAttributeInt("positionX") ?? 0;
            layer.positionY = node.GetAttributeInt("positionY") ?? 0;
            return layer;
        }
    }
}
