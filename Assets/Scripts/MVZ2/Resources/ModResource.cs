using System.Collections.Generic;
using System.Xml;
using MVZ2.Rendering;
using MVZ2.Talk;
using UnityEngine;

namespace MVZ2.Resources
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
        public NoteMetaList NoteMetaList { get; set; }
        public Dictionary<string, AudioClip> Sounds = new();
        public Dictionary<string, AudioClip> Musics = new();
        public Dictionary<string, Model> Models = new();
        public Dictionary<string, Sprite> ModelIcons = new();
        public Dictionary<string, Sprite[]> SpriteSheets = new();
        public Dictionary<string, Sprite> Sprites = new();
        public Dictionary<string, CharacterVariantSprite[]> CharacterVariantSprites = new();
        public Dictionary<string, TalkMeta> TalkMetas = new();
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
                    EntityMetaList = EntityMetaList.FromXmlNode(document["entities"], defaultNsp);
                    break;
                case "almanac":
                    AlmanacMetaList = AlmanacMetaList.FromXmlNode(document["almanac"], defaultNsp);
                    break;
                case "notes":
                    NoteMetaList = NoteMetaList.FromXmlNode(document["notes"], defaultNsp);
                    break;
            }
        }
    }
}
