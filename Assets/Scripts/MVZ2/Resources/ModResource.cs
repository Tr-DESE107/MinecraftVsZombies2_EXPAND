using System.Collections.Generic;
using System.Xml;
using PVZEngine;
using UnityEngine;

namespace MVZ2
{
    public class ModResource
    {
        public string Namespace { get; set; }
        public SoundsMeta SoundMeta { get; set; }
        public Dictionary<string, AudioClip> AudioClips { get; set; }
        public ModelsMeta ModelMeta { get; set; }
        public Dictionary<string, Model> Models { get; set; }
        public Dictionary<string, Sprite> ModelIcons { get; set; }
        public Dictionary<string, Sprite[]> SpriteSheets { get; set; }
        public Dictionary<string, Sprite> Sprites { get; set; }
        public FragmentsMeta FragmentsMeta { get; set; }
        public ModResource(string spaceName)
        {
            Namespace = spaceName;
        }
    }
}
