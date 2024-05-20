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
        public ModResource(string spaceName)
        {
            Namespace = spaceName;
        }
    }
}
