﻿using System.Collections.Generic;
using MVZ2.Level;
using MVZ2.Map;
using MVZ2.Metas;
using MVZ2.Models;
using MVZ2.TalkData;
using UnityEngine;

namespace MVZ2.Modding
{
    public class ModResource
    {
        public string Namespace { get; set; }
        public SoundMetaList SoundMetaList { get; set; }
        public ModelMetaList ModelMetaList { get; set; }
        public FragmentMetaList FragmentMetaList { get; set; }
        public TalkCharacterMetaList TalkCharacterMetaList { get; set; }
        public DifficultyMetaList DifficultyMetaList { get; set; }
        public ArmorMetaList ArmorMetaList { get; set; }
        public EntityMetaList EntityMetaList { get; set; }
        public ShapeMetaList ShapeMetaList { get; set; }
        public ArtifactMetaList ArtifactMetaList { get; set; }
        public AlmanacMetaList AlmanacMetaList { get; set; }
        public NoteMetaList NoteMetaList { get; set; }
        public StageMetaList StageMetaList { get; set; }
        public MapMetaList MapMetaList { get; set; }
        public AreaMetaList AreaMetaList { get; set; }
        public StatMetaList StatMetaList { get; set; }
        public AchievementMetaList AchievementMetaList { get; set; }
        public StoreMetaList StoreMetaList { get; set; }
        public MusicMetaList MusicMetaList { get; set; }
        public ArchiveMetaList ArchiveMetaList { get; set; }
        public MainmenuViewMetaList MainmenuViewMetaList { get; set; }
        public ProgressBarMetaList ProgressBarMetaList { get; set; }
        public BlueprintMetaList BlueprintMetaList { get; set; }
        public SpawnMetaList SpawnMetaList { get; set; }
        public GridMetaList GridMetaList { get; set; }
        public CreditMetaList CreditsMetaList { get; set; }
        public ChapterTransitionMetaList ChapterTransitionMetaList { get; set; }
        public ArcadeMetaList ArcadeMetaList { get; set; }
        public Dictionary<string, AudioClip> Sounds = new();
        public Dictionary<string, AudioClip> Musics = new();
        public Dictionary<string, Sprite[]> SpriteSheets = new();
        public Dictionary<string, Sprite> Sprites = new();
        public Dictionary<string, CharacterVariantSprite[]> CharacterVariantSprites = new();
        public Dictionary<string, TalkMeta> TalkMetas = new();
        public Dictionary<string, MapModel> MapModels = new();
        public Dictionary<string, AreaModel> AreaModels = new();
        public Dictionary<string, Model> Models = new();
        public Dictionary<string, Sprite> ModelIcons = new();
        public ModResource(string spaceName)
        {
            Namespace = spaceName;
        }
    }
}
