using System.Collections.Generic;
using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ModResource
    {
        public string Namespace { get; set; }
        public SoundMetaList SoundMetaList { get; set; }
        public ModelMetaList ModelMetaList { get; set; }
        public FragmentMetaList FragmentsMetaList { get; set; }
        public Dictionary<string, AudioClip> Sounds = new();
        public Dictionary<string, Model> Models = new();
        public Dictionary<string, Sprite> ModelIcons = new();
        public Dictionary<string, Sprite[]> SpriteSheets = new();
        public Dictionary<string, Sprite> Sprites = new();
        public ModResource(string spaceName)
        {
            Namespace = spaceName;
        }
    }
}
