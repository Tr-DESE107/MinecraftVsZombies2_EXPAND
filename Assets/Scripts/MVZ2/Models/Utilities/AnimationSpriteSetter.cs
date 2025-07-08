using MVZ2.Managers;
using UnityEngine;

namespace MVZ2.Models
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public class AnimationSpriteSetter : MonoBehaviour
    {
        public void Update()
        {
            if (sprites != null && sprites.Length > 0)
            {
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
            index = Mathf.Clamp(i, 0, sprites.Length - 1);
            beforeIndex = index;
            Sprite sprite = sprites[index];
            if (Main)
            {
                sprite = Main.GetFinalSprite(sprite);
            }
            Renderer.sprite = sprite;
        }
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
        public MainManager Main => MainManager.Instance;
        private SpriteRenderer sprRenderer;
        public Sprite[] sprites;
        private int beforeIndex = -1;
        [SerializeField]
        private int index = 0;
    }
}