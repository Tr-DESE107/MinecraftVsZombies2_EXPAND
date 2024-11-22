using System.Collections.Generic;
using System.Linq;
using System.Xml;
using PVZEngine;

namespace MVZ2Logic.Talk
{
    public class TalkCharacterMetaList
    {
        public List<TalkCharacterMeta> metas = new List<TalkCharacterMeta>();
    }

    public class TalkCharacterMeta
    {
        public string name;
        public NamespaceID unlockCondition;
        public List<TalkCharacterVariant> variants = new List<TalkCharacterVariant>();

        public TalkCharacterVariant GetVariant(NamespaceID id)
        {
            return variants.FirstOrDefault(v => v.id == id);
        }
    }

    public class TalkCharacterVariant
    {
        public NamespaceID id;
        public int? width;
        public int? height;
        public float? pivotX;
        public float? pivotY;
        public NamespaceID parent;
        public List<TalkCharacterLayer> layers = new List<TalkCharacterLayer>();
    }

    public class TalkCharacterLayer
    {
        public SpriteReference sprite;
        public int positionX;
        public int positionY;
    }
}
