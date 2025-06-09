using UnityEngine;

namespace MVZ2.Models
{
    [RequireComponent(typeof(SpriteRenderer))]
    public abstract class SpriteSetter : ModelComponent
    {
        public override void UpdateFrame(float deltaTime)
        {
            if (sprites != null && sprites.Length > 0)
            {
                var index = GetIndex();
                if (index != beforeIndex || Application.isEditor)
                {
                    SetSpriteIndex(index);
                }
            }
        }
        public void SetSpritePercent(float percent)
        {
            SetSpriteIndex(Mathf.FloorToInt(percent * sprites.Length));
        }

        public void SetSpriteIndex(int i)
        {
            i = Mathf.Clamp(i, 0, sprites.Length - 1);
            beforeIndex = i;
            Sprite sprite = sprites[i];
            if (Main)
            {
                sprite = Main.GetFinalSprite(sprite);
            }
            Renderer.sprite = sprite;
        }
        public abstract int GetIndex();
        private SpriteRenderer Renderer
        {
            get
            {
                if (!sprRenderer)
                {
                    sprRenderer = GetComponent<SpriteRenderer>();
                }
                return sprRenderer;
            }
        }
        private SpriteRenderer sprRenderer;
        public Sprite[] sprites;
        private int beforeIndex = -1;
    }
}