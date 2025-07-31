using UnityEngine;

namespace MVZ2.Entities
{
    public class HeightIndicatorController : MonoBehaviour
    {
        public void SetHeight(float value)
        {
            indicatorRenderer.size = new Vector2(indicatorRenderer.size.x, value);
        }
        [SerializeField]
        private SpriteRenderer indicatorRenderer;
    }
}