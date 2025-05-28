﻿using System;
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
        public void AddSprite(string category, Sprite sprite, Sprite background)
        {
            var cat = categories.FirstOrDefault(c => c.name == category);
            if (cat == null)
            {
                cat = new GeneratedSpriteCategory(category);
                categories.Add(cat);
            }
            cat.AddSprite(sprite, background);
        }
        public void RemoveSprite(string category, string name)
        {
            var cat = categories.FirstOrDefault(c => c.name == category);
            if (cat == null)
                return;
            cat.RemoveSprite(name);
            if (cat.Count <= 0)
            {
                categories.Remove(cat);
            }
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
        public void AddSprite(Sprite sprite, Sprite preview)
        {
            sprites.Add(new SpritePreview(sprite, preview));
        }
        public void RemoveSprite(string name)
        {
            sprites.RemoveAll(p => p.name == name);
        }
        public int Count => sprites.Count;
        public string name;
        [SerializeField]
        private List<SpritePreview> sprites = new List<SpritePreview>();
    }
    [Serializable]
    public class SpritePreview
    {
        public SpritePreview(Sprite sprite, Sprite background)
        {
            this.name = sprite.name;
            this.sprite = sprite;
            this.background = background;
        }
        public string name;
        public Sprite background;
        public Sprite sprite;
    }
}
