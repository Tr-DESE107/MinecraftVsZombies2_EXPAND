using System.Collections.Generic;
using MVZ2.Entities;
using MVZ2.Grids;
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
        public void SetHeldItem(HeldItemDefinition definition, long id)
        {
            heldItemDefinition = definition;
            heldItemId = id;
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
                m_Hits = Physics2D.GetRayIntersectionAll(ray, distanceToClipPlane, finalEventMask);
                hitCount = m_Hits.Length;
            }
            else
            {
                if (m_LastMaxRayIntersections != m_MaxRayIntersections)
                {
                    m_Hits = new RaycastHit2D[maxRayIntersections];
                    m_LastMaxRayIntersections = m_MaxRayIntersections;
                }

                hitCount = Physics2D.GetRayIntersectionNonAlloc(ray, m_Hits, distanceToClipPlane, finalEventMask);
            }

            if (hitCount == 0)
                return;

            for (int b = 0, bmax = hitCount; b < bmax; ++b)
            {
                var hit = m_Hits[b];
                var go = hit.collider.gameObject;

                if (!IsGameObjectValid(go))
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
                    index = resultAppendList.Count
                };

                resultAppendList.Add(result);
            }
        }
        private bool IsGameObjectValid(GameObject go)
        {
            var receiver = go.GetComponentInParent<ILevelRaycastReceiver>();
            if (receiver == null)
                return false;
            return receiver.IsValidReceiver(level, heldItemDefinition, heldItemId);
        }
        public override int sortOrderPriority => 0;
        RaycastHit2D[] m_Hits;
        private HeldItemDefinition heldItemDefinition;
        private long heldItemId;
        private LevelEngine level;
    }
}
