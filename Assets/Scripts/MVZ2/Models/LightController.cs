using UnityEngine;

namespace MVZ2.Models
{
    public class LightController : MonoBehaviour
    {
        public void SetRange(Vector2 scale, Vector2 randomOffset)
        {
            this.scale = scale;
            var lightScale = scale + randomOffset;
            var aspect = lightScale.x / lightScale.y;
            if (lightScale.x > lightScale.y)
            {
                lightRenderer.transform.localScale = new Vector3(lightScale.y, lightScale.y);
                lightRenderer.size = new Vector2(aspect, 1);
            }
            else
            {
                lightRenderer.transform.localScale = new Vector3(lightScale.x, lightScale.x);
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
            SetRange(serializable.scale, Vector2.zero);
            SetColor(serializable.color);
        }
        [SerializeField]
        private SpriteRenderer lightRenderer;
        private Vector2 scale;
        private Color color;
    }
}
