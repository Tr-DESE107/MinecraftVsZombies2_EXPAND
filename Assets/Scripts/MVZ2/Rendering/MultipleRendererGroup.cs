using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Rendering
{
    [RequireComponent(typeof(SortingGroup))]
    public class MultipleRendererGroup : MonoBehaviour
    {
        #region 公有方法
        public void SetSimulationSpeed(float speed)
        {
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.simulationSpeed = speed;
            }
        }
        public void UpdateRendererElements()
        {
            elements.Clear();
            foreach (var renderer in GetComponentsInChildren<Renderer>(true))
            {
                if (!IsChildOfGroup(renderer.transform, this))
                    continue;
                if (renderer is SpriteMask)
                    continue;
                var element = renderer.GetComponent<RendererElement>();
                if (!element)
                {
                    element = renderer.gameObject.AddComponent<RendererElement>();
                }
                elements.Add(element);
            }

            particles.Clear();
            foreach (var ps in GetComponentsInChildren<ParticleSystem>(true))
            {
                if (!IsParticleChildOfGroup(ps.transform, this))
                    continue;
                particles.Add(ps);
            }
        }
        public void SetGroundPosition(Vector3 position)
        {
            foreach (var element in elements)
            {
                if (!element.LockToGround)
                    continue;
                var trans = element.transform;
                trans.position = position;
            }
        }
        public void SetPropertyInt(string name, int value)
        {
            foreach (Renderer renderer in GetAllRenderers())
            {
                SetRendererInt(renderer, name, value);
            }
        }

        public void SetPropertyFloat(string name, float alpha)
        {
            foreach (Renderer renderer in GetAllRenderers())
            {
                SetRendererFloat(renderer, name, alpha);
            }
        }

        public void SetPropertyColor(string name, Color color)
        {
            foreach (Renderer renderer in GetAllRenderers())
            {
                SetRendererColor(renderer, name, color);
            }
        }
        public void SetTint(Color color)
        {
            SetPropertyColor("_Color", color);
        }
        public void SetColorOffset(Color color)
        {
            SetPropertyColor("_ColorOffset", color);
        }
        #endregion

        #region 私有方法
        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }
        private static bool IsChildOfGroup(Transform child, MultipleRendererGroup group)
        {
            return child.GetComponentInParent<MultipleRendererGroup>() == group;
        }
        private static bool IsParticleChildOfGroup(Transform child, MultipleRendererGroup group)
        {
            return !child.parent.GetComponentInParent<ParticleSystem>() && IsChildOfGroup(child, group);
        }
        private void SetRendererFloat(Renderer renderer, string name, float alpha)
        {
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat(name, alpha);
            renderer.SetPropertyBlock(propertyBlock);
        }

        private void SetRendererColor(Renderer renderer, string name, Color color)
        {
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor(name, color);
            renderer.SetPropertyBlock(propertyBlock);
        }

        private void SetRendererInt(Renderer renderer, string name, int value)
        {
            propertyBlock.Clear();
            renderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetInt(name, value);
            renderer.SetPropertyBlock(propertyBlock);
        }
        private Renderer[] GetAllRenderers(bool includeExcluded = false)
        {
            if (includeExcluded)
                return elements.Select(e => e.Renderer).ToArray();
            return elements.Where(e => !e.ExcludedInGroup).Select(e => e.Renderer).ToArray();
        }
        #endregion
        public int SortingLayerID { get => sortingGroup.sortingLayerID; set => sortingGroup.sortingLayerID = value; }

        public string SortingLayerName { get => sortingGroup.sortingLayerName; set => sortingGroup.sortingLayerName = value; }

        public int SortingOrder { get => sortingGroup.sortingOrder; set => sortingGroup.sortingOrder = value; }

        [SerializeField]
        private List<RendererElement> elements = new List<RendererElement>();
        [SerializeField]
        private List<ParticleSystem> particles = new List<ParticleSystem>();
        [SerializeField]
        private SortingGroup sortingGroup;
        private MaterialPropertyBlock propertyBlock;

    }
}