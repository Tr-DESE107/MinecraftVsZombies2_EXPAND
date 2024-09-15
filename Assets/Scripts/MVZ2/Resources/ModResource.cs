using System.Collections.Generic;
using MVZ2.Talk;
using PVZEngine;
using UnityEngine;

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
    }
}
