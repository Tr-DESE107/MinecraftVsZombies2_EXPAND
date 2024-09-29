using UnityEngine;

namespace MVZ2.Rendering
{
    public class LightController : MonoBehaviour
    {
        public void SetRange(Vector2 scale)
        {
            this.scale = scale;
            var aspect = scale.x / scale.y;
            if (scale.x > scale.y)
            {
                lightRenderer.transform.localScale = new Vector3(scale.y, scale.y);
                lightRenderer.size = new Vector2(aspect, 1);
            }
            else
            {
                lightRenderer.transform.localScale = new Vector3(scale.x, scale.x);
                lightRenderer.size = new Vector2(1, 1 / aspect);
            }
        }
        public void SetColor(Color color)
        {
            this.color = color;
            lightRenderer.color = color;
        }
        public SerializableLightController ToSerializable()
        {
            return new SerializableLightController()
            {
                scale = scale,
                color = color,
            };
        }
        public void LoadFromSerializable(SerializableLightController serializable)
        {
            if (serializable == null)
                return;
            SetRange(serializable.scale);
            SetColor(serializable.color);
        }
        [SerializeField]
        private SpriteRenderer lightRenderer;
        private Vector2 scale;
        private Color color;
    }
}
