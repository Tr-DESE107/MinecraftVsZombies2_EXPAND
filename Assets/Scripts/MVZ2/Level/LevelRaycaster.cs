﻿using System.Collections.Generic;
using MVZ2.HeldItems;
using MVZ2Logic.HeldItems;
using PVZEngine.Level;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MVZ2.Level
{
    /// <summary>
    /// Simple event system using physics raycasts.
    /// </summary>
    [AddComponentMenu("Event/Level Raycaster")]
    [RequireComponent(typeof(Camera))]
    /// <summary>
    /// Raycaster for casting against 2D Physics components.
    /// </summary>
    public class LevelRaycaster : Physics2DRaycaster
    {
        protected LevelRaycaster()
        { }

        public void Init(LevelEngine level)
        {
            this.level = level;
        }
        public void SetHeldItem(HeldItemDefinition definition, IHeldItemData data, float radius)
        {
            heldItemDefinition = definition;
            heldItemData = data;
            castRadius = radius;
        }
        /// <summary>
        /// Raycast against 2D elements in the scene.
        /// </summary>
        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            Ray ray = new Ray();
            float distanceToClipPlane = 0;
            int displayIndex = 0;
            if (!ComputeRayAndDistance(eventData, ref ray, ref displayIndex, ref distanceToClipPlane))
                return;

            int hitCount = 0;

            if (maxRayIntersections == 0)
            {
                if (castRadius <= 0)
                {
                    m_Hits = Physics2D.GetRayIntersectionAll(ray, distanceToClipPlane, finalEventMask);
                }
                else
                {
                    var hits = Physics2D.CircleCastAll(ray.origin, castRadius, ray.direction, distanceToClipPlane, finalEventMask);
                    m_Hits = ProcessCircleCasts(hits, ray.origin, distanceToClipPlane);
                }
                hitCount = m_Hits.Length;
            }
            else
            {
                if (m_LastMaxRayIntersections != m_MaxRayIntersections)
                {
                    m_Hits = new RaycastHit2D[maxRayIntersections];
                    m_LastMaxRayIntersections = m_MaxRayIntersections;
                }

                if (castRadius <= 0)
                {
                    hitCount = Physics2D.GetRayIntersectionNonAlloc(ray, m_Hits, distanceToClipPlane, finalEventMask);
                }
                else
                {
                    hitCount = Physics2D.CircleCastNonAlloc(ray.origin, castRadius, ray.direction, m_Hits, distanceToClipPlane, finalEventMask);
                    hitCount = ProcessCircleCastsNonAlloc(m_Hits, hitCount, ray.origin, distanceToClipPlane);
                }
            }

            if (hitCount == 0)
                return;

            for (int b = 0, bmax = hitCount; b < bmax; ++b)
            {
                var hit = m_Hits[b];
                var go = hit.collider.gameObject;

                var receiver = go.GetComponentInParent<ILevelRaycastReceiver>();
                if (receiver == null)
                    continue;
                if (!receiver.IsValidReceiver(level, heldItemDefinition, heldItemData, eventData))
                    continue;

                var result = new RaycastResult
                {
                    gameObject = go,
                    module = this,
                    distance = hit.distance,
                    worldPosition = hit.point,
                    worldNormal = hit.normal,
                    screenPosition = eventData.position,
                    displayIndex = displayIndex,
                    index = resultAppendList.Count,
                    sortingLayer = receiver.GetSortingLayer(),
                    sortingOrder = receiver.GetSortingOrder(),
                };

                resultAppendList.Add(result);
            }
        }
        private RaycastHit2D[] ProcessCircleCasts(RaycastHit2D[] hits, Vector3 origin, float maxDistance)
        {
            var hitList = new List<RaycastHit2D>();
            for (int i = 0; i < hits.Length; i++)
            {
                var hit = hits[i];
                // 获取碰撞体上最近点（2D坐标）
                Vector2 closestPoint = Physics2D.ClosestPoint(origin, hit.collider);

                // 转换为3D点
                Vector3 hitPoint3D = new Vector3(closestPoint.x, closestPoint.y, hit.collider.transform.position.z);

                // 计算真实3D距离
                float distance = Vector3.Distance(origin, hitPoint3D);
                hit.distance = distance;
                hit.fraction = distance / maxDistance;
                if (distance > maxDistance)
                    continue;
                hitList.Add(hit);
            }
            return hitList.ToArray();
        }
        private int ProcessCircleCastsNonAlloc(RaycastHit2D[] hits, int count, Vector3 origin, float maxDistance)
        {
            int finalCount = count;
            for (int i = 0; i < count; i++)
            {
                var hit = hits[i];
                // 获取碰撞体上最近点（2D坐标）
                Vector2 closestPoint = Physics2D.ClosestPoint(origin, hit.collider);

                // 转换为3D点
                Vector3 hitPoint3D = new Vector3(closestPoint.x, closestPoint.y, hit.collider.transform.position.z);

                // 计算真实3D距离
                float distance = Vector3.Distance(origin, hitPoint3D);
                hit.distance = distance;
                hit.fraction = distance / maxDistance;
                if (distance > maxDistance)
                {
                    for (int j = i; j < count - 1; j++)
                    {
                        hits[j] = hits[j + 1];
                    }
                    finalCount--;
                }
            }
            return finalCount;
        }
        RaycastHit2D[] m_Hits;
        private HeldItemDefinition heldItemDefinition;
        private IHeldItemData heldItemData;
        private float castRadius;
        private LevelEngine level;
    }
}
