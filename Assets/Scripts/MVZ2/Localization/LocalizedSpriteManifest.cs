﻿namespace MVZ2.Localization
{
    public class LocalizedSpriteManifest
    {
        public LocalizedSprite[] sprites;
        public LocalizedSpriteSheet[] spritesheets;
    }
    public class LocalizedSprite
    {
        public string name;
        public string texture;
        public float pivotX;
        public float pivotY;
    }
    public class LocalizedSpriteSheet
    {
        public string name;
        public string texture;
        public LocalizedSpriteSheetSlice[] slices;
    }
    public class LocalizedSpriteSheetSlice
    {
        public float x;
        public float y;
        public float width;
        public float height;
        public float pivotX;
        public float pivotY;
    }
}
