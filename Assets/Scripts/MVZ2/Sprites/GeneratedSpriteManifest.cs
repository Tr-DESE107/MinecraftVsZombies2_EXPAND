using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVZ2.Sprites
{
    [CreateAssetMenu(fileName = "NewGeneratedSpriteManifest", menuName = "MVZ2/Generated Sprite Manifest")]
    public class GeneratedSpriteManifest : ScriptableObject
    {
        public void Reset()
        {
            categories.Clear();
        }
        public void AddSprite(string category, Sprite sprite)
        {
            var cat = categories.FirstOrDefault(c => c.name == category);
            if (cat == null)
            {
                cat = new GeneratedSpriteCategory(category);
                categories.Add(cat);
            }
            cat.AddSprite(sprite);
        }
        [SerializeField]
        private List<GeneratedSpriteCategory> categories = new List<GeneratedSpriteCategory>();
    }
    [Serializable]
    public class GeneratedSpriteCategory
    {
        public GeneratedSpriteCategory(string name)
        {
            this.name = name;
        }
        public void AddSprite(Sprite sprite)
        {
            sprites.Add(new SpritePreview(sprite));
        }
        public string name;
        [SerializeField]
        private List<SpritePreview> sprites = new List<SpritePreview>();
    }
    [Serializable]
    public class SpritePreview
    {
        public SpritePreview(Sprite sprite)
        {
            this.name = sprite.name;
            this.sprite = sprite;
        }
        public string name;
        [SerializeField]
        private Sprite sprite;
    }
}
