using System.Collections.Generic;
using System.Linq;
using Codice.CM.Common;
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
                particle.SetSimulationSpeed(speed);
            }
        }
        public void SetLight(bool visible, Vector2 range, Color color)
        {
            lightController.gameObject.SetActive(visible);
            lightController.SetColor(color);
            lightController.SetRange(range);
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
                if (renderer.gameObject.layer == Layers.LIGHT)
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
                var player = ps.GetComponent<ParticlePlayer>();
                if (!player)
                {
                    player = ps.gameObject.AddComponent<ParticlePlayer>();
                }
                particles.Add(player);
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
            foreach (var element in GetAllElements())
            {
                element.SetInt(name, value);
            }
        }

        public void SetPropertyFloat(string name, float alpha)
        {
            foreach (var element in GetAllElements())
            {
                element.SetFloat(name, alpha);
            }
        }

        public void SetPropertyColor(string name, Color color)
        {
            foreach (var element in GetAllElements())
            {
                element.SetColor(name, color);
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

        public SerializableMultipleRendererGroup ToSerializable()
        {
            var serializable = new SerializableMultipleRendererGroup();
            serializable.sortingLayerID = SortingLayerID;
            serializable.sortingOrder = SortingOrder;
            serializable.elements = elements.Select(e => e.ToSerializable()).ToArray();
            serializable.particles = particles.Select(e => e.ToSerializable()).ToArray();
            serializable.light = lightController.ToSerializable();
            return serializable;
        }
        public void LoadFromSerializable(SerializableMultipleRendererGroup serializable)
        {
            SortingLayerID = serializable.sortingLayerID;
            SortingOrder = serializable.sortingOrder;
            for (int i = 0; i < elements.Count; i++)
            {
                if (i >= serializable.elements.Length)
                    break;
                var element = elements[i];
                var data = serializable.elements[i];
                element.LoadFromSerializable(data);
            }
            for (int i = 0; i < particles.Count; i++)
            {
                if (i >= serializable.particles.Length)
                    break;
                var particle = particles[i];
                var data = serializable.particles[i];
                particle.LoadFromSerializable(data);
            }
            lightController.LoadFromSerializable(serializable.light);
        }
        #endregion

        #region 私有方法
        private static bool IsChildOfGroup(Transform child, MultipleRendererGroup group)
        {
            return child.GetComponentInParent<MultipleRendererGroup>() == group;
        }
        private static bool IsParticleChildOfGroup(Transform child, MultipleRendererGroup group)
        {
            return !child.parent.GetComponentInParent<ParticleSystem>() && IsChildOfGroup(child, group);
        }
        private RendererElement[] GetAllElements(bool includeExcluded = false)
        {
            if (includeExcluded)
                return elements.ToArray();
            return elements.Where(e => !e.ExcludedInGroup).ToArray();
        }
        #endregion
        public int SortingLayerID { get => sortingGroup.sortingLayerID; set => sortingGroup.sortingLayerID = value; }

        public string SortingLayerName { get => sortingGroup.sortingLayerName; set => sortingGroup.sortingLayerName = value; }

        public int SortingOrder { get => sortingGroup.sortingOrder; set => sortingGroup.sortingOrder = value; }

        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private LightController lightController;
        [SerializeField]
        private List<RendererElement> elements = new List<RendererElement>();
        [SerializeField]
        private List<ParticlePlayer> particles = new List<ParticlePlayer>();

    }
    public class SerializableMultipleRendererGroup
    {
        public SerializableRendererElement[] elements;
        public SerializableParticleSystem[] particles;
        public SerializableLightController light;
        public int sortingLayerID;
        public int sortingOrder;
    }
}