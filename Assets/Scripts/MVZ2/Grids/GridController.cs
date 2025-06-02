using System;
using MVZ2.HeldItems;
using MVZ2.Level;
using MVZ2.Managers;
using MVZ2Logic;
using MVZ2Logic.HeldItems;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Grids
{
    public class GridController : MonoBehaviour, ILevelRaycastReceiver
    {
        public void UpdateFixed()
        {
            holdStreakHandler.UpdateHoldAndStreak();
        }
        public void UpdateGridView(GridViewData viewData)
        {
            transform.localPosition = viewData.position;
            spriteRenderer.sprite = viewData.sprite;
            SetBevel(viewData.slope);

            var sprSize = spriteRenderer.size;
            sprSize.y = size.y + Mathf.Abs(BevelHeight);
            spriteRenderer.size = sprSize;
        }
        public void SetColor(Color color)
        {
            spriteRenderer.color = color;
        }
        public void SetDisplaySection(float start, float end)
        {
            var sprSize = spriteRenderer.size;
            sprSize.y = (end - start) * size.y + Mathf.Abs(BevelHeight);
            spriteRenderer.size = sprSize;

            var pos = rendererTransform.localPosition;
            pos.y = (end - 1) * size.y;
            rendererTransform.localPosition = pos;
        }
        public void SetBevel(float height)
        {
            var points = polygonCollider.points;
            if (points.Length >= 4)
            {
                points[2].y = -size.y * 0.5f - height;
                points[3].y = size.y * 0.5f - height;
                polygonCollider.points = points;
            }
            BevelHeight = height;
        }
        public Vector2 TransformWorld2ColliderPosition(Vector3 worldPosition)
        {
            var pos2D = (Vector2)transform.position;
            var lossyScale = (Vector2)transform.lossyScale;
            var lossySize = Vector2.Scale(size, lossyScale);
            var slope = BevelHeight / size.x;

            var origin = pos2D - lossySize * 0.5f;
            var relativeWorldPos = (Vector2)worldPosition - origin;
            var colliderX = relativeWorldPos.x / lossySize.x;

            var yOffset = slope * relativeWorldPos.x;
            var colliderY = (relativeWorldPos.y + yOffset) / lossySize.y;

            return new Vector2(colliderX, colliderY);
        }
        private void Awake()
        {
            holdStreakHandler.OnPointerInteraction += (_, d, i) => OnPointerInteraction?.Invoke(this, d, i);
        }
        bool ILevelRaycastReceiver.IsValidReceiver(LevelEngine level, HeldItemDefinition definition, IHeldItemData data, PointerEventData eventData)
        {
            if (definition == null)
                return false;
            var target = new HeldItemTargetGrid(level.GetGrid(Column, Lane), TransformWorld2ColliderPosition(eventData.pointerCurrentRaycast.worldPosition));
            var pointer = InputManager.GetPointerDataFromEventData(eventData);
            return definition.IsValidFor(target, data, pointer);
        }
        int ILevelRaycastReceiver.GetSortingLayer()
        {
            return spriteRenderer.sortingLayerID;
        }
        int ILevelRaycastReceiver.GetSortingOrder()
        {
            return spriteRenderer.sortingOrder;
        }
        public event Action<GridController, PointerEventData, PointerInteraction> OnPointerInteraction;
        public int Lane { get; set; }
        public int Column { get; set; }
        public float BevelHeight { get; private set; }
        [SerializeField]
        private Vector2 size;
        [SerializeField]
        private Transform rendererTransform;
        [SerializeField]
        private SpriteRenderer spriteRenderer;
        [SerializeField]
        private LevelPointerInteractionHandler holdStreakHandler;
        [SerializeField]
        private PolygonCollider2D polygonCollider;
    }
    public struct GridViewData
    {
        public Vector2 position;
        public Sprite sprite;
        public float slope;
    }
}
