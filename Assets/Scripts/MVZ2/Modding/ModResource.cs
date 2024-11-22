using System.Collections.Generic;
using System.Xml;
using MVZ2.Vanilla.Entities;
using MVZ2Logic.Almanacs;
using MVZ2Logic.Audios;
using MVZ2Logic.Entities;
using MVZ2Logic.Level;
using MVZ2Logic.Map;
using MVZ2Logic.Models;
using MVZ2Logic.Notes;
using MVZ2Logic.Talk;
using UnityEngine;

namespace MVZ2Logic.Modding
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
        public StageMetaList StageMetaList { get; set; }
        public MapMetaList MapMetaList { get; set; }
        public AreaMetaList AreaMetaList { get; set; }
        public Dictionary<string, AudioClip> Sounds = new();
        public Dictionary<string, AudioClip> Musics = new();
        public Dictionary<string, Sprite[]> SpriteSheets = new();
        public Dictionary<string, Sprite> Sprites = new();
        public Dictionary<string, CharacterVariantSprite[]> CharacterVariantSprites = new();
        public Dictionary<string, TalkMeta> TalkMetas = new();
        public Dictionary<string, IMapModel> MapModels = new();
        public Dictionary<string, IAreaModel> AreaModels = new();
        public Dictionary<string, IModel> Models = new();
        public Dictionary<string, Sprite> ModelIcons = new();
        public ModResource(string spaceName)
        {
            Namespace = spaceName;
        }
    }
}
