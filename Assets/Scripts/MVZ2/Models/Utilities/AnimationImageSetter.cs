using MVZ2.Managers;
using UnityEngine;
using UnityEngine.UI;

namespace Rendering
{
    [RequireComponent(typeof(Image))]
    [ExecuteAlways]
    public class AnimationImageSetter : MonoBehaviour
    {
        private void OnEnable()
        {
            SetSpriteIndex(index);
        }
        private void LateUpdate()
        {
            if (sprites != null && sprites.Length > 0)
            {
                if (index != beforeIndex)
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
            if (sprites == null)
                return;
            index = Mathf.Clamp(i, 0, sprites.Length - 1);
            beforeIndex = index;
            Sprite sprite = sprites[index];
            if (Main)
            {
                sprite = Main.GetFinalSprite(sprite);
            }
            Renderer.sprite = sprite;
        }
        private Image Renderer
        {
            get
            {
                if (!image)
                {
                    image = GetComponent<Image>();
                }
                return image;
            }
        }
        private MainManager Main => MainManager.Instance;
        private Image image;
        public Sprite[] sprites;
        private int beforeIndex = -1;
        [SerializeField]
        private int index = 0;
    }
}