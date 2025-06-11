﻿using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class PanZoomController : MonoBehaviour, IDragHandler, IScrollHandler
    {
        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            originalScale = Vector3.one;
            originalPosition = Vector3.zero;
            targetPosition = originalPosition;
            ResetView();
        }

        private void Update()
        {
            // 处理双指触控
            HandleTouchInput();

            // 平滑移动
            if (smoothPan && rectTransform.localPosition != targetPosition)
            {
                rectTransform.localPosition = Vector3.SmoothDamp(
                    rectTransform.localPosition,
                    targetPosition,
                    ref velocity,
                    smoothTime
                );
                if (enableConstraints)
                {
                    ApplyMovementConstraints();
                }
            }
        }

        // 处理鼠标拖拽
        public void OnDrag(PointerEventData eventData)
        {
            // 只响应鼠标左键或单指触控
            if (eventData.pointerId == -1 || eventData.pointerId == 0)
            {
                var localPos1 = ScreenToLocalPoint(eventData.position - eventData.delta);
                var localPos2 = ScreenToLocalPoint(eventData.position);
                var localDelta = localPos2 - localPos1;
                Vector2 delta = localDelta * panSensitivity;
                targetPosition += (Vector3)delta;
                if (!smoothPan)
                {
                    rectTransform.localPosition = targetPosition;
                }
                if (enableConstraints)
                {
                    ApplyMovementConstraints();
                }
            }
        }

        // 处理鼠标滚轮缩放
        public void OnScroll(PointerEventData eventData)
        {
            Zoom(eventData.scrollDelta.y * zoomSpeed, eventData.position);
        }

        // 处理触屏输入
        private void HandleTouchInput()
        {
            // 双指触控
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                // 初始状态
                if (touch2.phase == TouchPhase.Began)
                {
                    initialDistance = Vector2.Distance(touch1.position, touch2.position);
                    initialScale = rectTransform.localScale;
                    initialMidpoint = (touch1.position + touch2.position) / 2;
                    initialPosition = rectTransform.localPosition;
                }

                // 移动中
                if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    // 计算当前距离和缩放比例
                    float currentDistance = Vector2.Distance(touch1.position, touch2.position);
                    float scaleFactor = currentDistance / initialDistance;
                    Vector3 newScale = initialScale * scaleFactor;

                    // 应用缩放限制
                    newScale = new Vector3(
                        Mathf.Clamp(newScale.x, minZoom, maxZoom),
                        Mathf.Clamp(newScale.y, minZoom, maxZoom),
                        1
                    );

                    rectTransform.localScale = newScale;

                    // 计算中点位置变化
                    Vector2 currentMidpoint = (touch1.position + touch2.position) / 2;
                    Vector2 midpointDelta = currentMidpoint - initialMidpoint;

                    // 根据缩放比例调整移动量
                    midpointDelta *= 1 / rectTransform.localScale.x;

                    // 更新位置
                    rectTransform.localPosition = initialPosition + (Vector3)midpointDelta;
                    targetPosition = rectTransform.localPosition;

                    if (enableConstraints)
                    {
                        ApplyMovementConstraints();
                    }
                }
            }
        }
        private Vector2 ScreenToLocalPoint(Vector2 position)
        {
            var canvas = rectTransform.GetRootCanvas();
            var camera = canvas.worldCamera;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rectTransform.parent as RectTransform,
                position,
                camera,
                out Vector2 localPoint
            );
            return localPoint;
        }

        // 缩放函数
        private void Zoom(float increment, Vector2 zoomCenter)
        {
            // 计算缩放前的局部坐标
            var localPoint = ScreenToLocalPoint(zoomCenter);

            // 计算缩放比例
            float newScale = Mathf.Clamp(
                rectTransform.localScale.x + increment,
                minZoom,
                maxZoom
            );

            // 应用缩放
            Vector3 oldScale = rectTransform.localScale;
            rectTransform.localScale = new Vector3(newScale, newScale, 1);

            // 计算缩放后位置偏移
            Vector3 scaleFactor = new Vector3(newScale / oldScale.x, newScale / oldScale.y, 1 / oldScale.z);
            Vector3 localPos = rectTransform.localPosition;
            localPos = Vector3.Scale(localPos - (Vector3)localPoint, scaleFactor) + (Vector3)localPoint;

            // 更新位置
            rectTransform.localPosition = localPos;
            targetPosition = rectTransform.localPosition;
            if (enableConstraints)
            {
                ApplyMovementConstraints();
            }
        }

        // 重置视图
        public void ResetView()
        {
            rectTransform.localScale = originalScale;
            rectTransform.localPosition = originalPosition;
            targetPosition = originalPosition;
            velocity = Vector3.zero;
            if (enableConstraints)
            {
                ApplyMovementConstraints();
            }
        }
        // 应用移动限制（实时）
        private void ApplyMovementConstraints()
        {
            ClampPosition(ref targetPosition);
        }
        private void ClampPosition(ref Vector3 position)
        {
            if (viewport == null)
                return;

            // 获取视区和内容的尺寸
            Vector2 viewportSize = viewport.rect.size;
            Vector2 contentSize = rectTransform.rect.size * rectTransform.localScale.x;

            // 计算边界
            float minX = (viewportSize.x - contentSize.x) * 0.5f + constraintPadding;
            float maxX = -minX;
            float minY = (viewportSize.y - contentSize.y) * 0.5f + constraintPadding;
            float maxY = -minY;

            // 当内容小于视区时，限制移动范围
            if (contentSize.x < viewportSize.x)
            {
                position.x = 0;
            }
            else
            {
                position.x = Mathf.Clamp(position.x, minX, maxX);
            }

            if (contentSize.y < viewportSize.y)
            {
                position.y = 0;
            }
            else
            {
                position.y = Mathf.Clamp(position.y, minY, maxY);
            }
        }
        [Header("Zoom Settings")]
        public float zoomSpeed = 0.1f;
        public float minZoom = 0.5f;
        public float maxZoom = 3f;

        [Header("Pan Settings")]
        public float panSensitivity = 1f;
        public bool smoothPan = true;
        public float smoothTime = 0.1f;

        [Header("Viewport Constraints")]
        public RectTransform viewport; // 视区矩形
        public bool enableConstraints = true; // 是否启用移动限制
        public float constraintPadding = 10f; // 边界内边距

        private RectTransform rectTransform;
        private Vector3 originalScale;
        private Vector3 originalPosition;
        private Vector3 targetPosition;
        private Vector3 velocity = Vector3.zero;

        // 用于触屏双指缩放
        private float initialDistance;
        private Vector3 initialScale;
        private Vector2 initialMidpoint;
        private Vector3 initialPosition;
    }
}