using System.Collections.Generic;
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
                    m_Hits = Physics2D.CircleCastAll(ray.origin, castRadius, ray.direction, distanceToClipPlane, finalEventMask);
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
        RaycastHit2D[] m_Hits;
        private HeldItemDefinition heldItemDefinition;
        private IHeldItemData heldItemData;
        private float castRadius;
        private LevelEngine level;
    }
}
