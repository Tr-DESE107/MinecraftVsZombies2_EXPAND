using System.Collections.Generic;
using MVZ2.Talk;
using PVZEngine;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;
using UnityEngine.Profiling.Memory.Experimental;
using System.Xml;

namespace MVZ2
{
    public class ModResource
    {
        public string Namespace { get; set; }
        public SoundMetaList SoundMetaList { get; set; }
        public ModelMetaList ModelMetaList { get; set; }
        public FragmentMetaList FragmentMetaList { get; set; }
        public TalkCharacterMetaList TalkCharacterMetaList { get; set; }
        public DifficultyMetaList DifficultyMetaList { get; set; }
        public EntityMetaList EntityMetaList { get; set; }
        public AlmanacMetaList AlmanacMetaList { get; set; }
        public Dictionary<NamespaceID, AudioClip> Sounds = new();
        public Dictionary<NamespaceID, AudioClip> Musics = new();
        public Dictionary<NamespaceID, Model> Models = new();
        public Dictionary<NamespaceID, Sprite> ModelIcons = new();
        public Dictionary<NamespaceID, Sprite[]> SpriteSheets = new();
        public Dictionary<NamespaceID, Sprite> Sprites = new();
        public Dictionary<NamespaceID, CharacterVariantSprite[]> CharacterVariantSprites = new();
        public Dictionary<NamespaceID, TalkMeta> TalkMetas = new();
        public ModResource(string spaceName)
        {
            Namespace = spaceName;
        }
        public void LoadMetaList(string metaPath, XmlDocument document, string defaultNsp)
        {
            switch (metaPath)
            {
                case "talkcharacters":
                    TalkCharacterMetaList = TalkCharacterMetaList.FromXmlNode(document["characters"], defaultNsp);
                    break;
                case "sounds":
                    SoundMetaList = SoundMetaList.FromXmlNode(document["sounds"], defaultNsp);
                    break;
                case "models":
                    ModelMetaList = ModelMetaList.FromXmlNode(document["models"], defaultNsp);
                    break;
                case "fragments":
                    FragmentMetaList = FragmentMetaList.FromXmlNode(document["fragments"]);
                    break;
                case "difficulties":
                    DifficultyMetaList = DifficultyMetaList.FromXmlNode(document["difficulties"]);
                    break;
                case "entities":
                    EntityMetaList = EntityMetaList.FromXmlNode(document["entities"]);
                    break;
                case "almanac":
                    AlmanacMetaList = AlmanacMetaList.FromXmlNode(document["almanac"], defaultNsp);
                    break;
            }
        }
    }
}
