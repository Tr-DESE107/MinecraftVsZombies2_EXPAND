using System;
using System.Collections.Generic;
using UnityEngine;

namespace MVZ2.Sprites
{
    [CreateAssetMenu(fileName = "NewSpriteManifest", menuName = "MVZ2/Sprite Manifest", order = 0)]
    public class SpriteManifest : ScriptableObject
    {
        public List<SpriteEntry> spriteEntries;
        public List<SpriteSheetEntry> spritesheetEntries;
    }
    [Serializable]
    public class SpriteEntry
    {
        public string name;
        public Sprite sprite;
    }
    [Serializable]
    public class SpriteSheetEntry
    {
        public string name;
        public Sprite[] spritesheet;
    }
}
