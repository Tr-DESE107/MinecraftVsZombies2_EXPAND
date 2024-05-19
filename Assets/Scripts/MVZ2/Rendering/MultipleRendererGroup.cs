using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Rendering
{
    [RequireComponent(typeof(SortingGroup))]
    public class MultipleRendererGroup : MonoBehaviour
    {
        private void Awake()
        {
            propertyBlock = new MaterialPropertyBlock();
        }

        public void SetPropertyInt(string name, int value)
        {
            foreach (Renderer renderer in renderers)
            {
                SetRendererInt(renderer, name, value);
            }
        }

        public void SetPropertyFloat(string name, float alpha)
        {
            foreach (Renderer renderer in renderers)
            {
                SetRendererFloat(renderer, name, alpha);
            }
        }

        public void SetPropertyColor(string name, Color color)
        {
            foreach (Renderer renderer in renderers)
            {
                SetRendererColor(renderer, name, color);
            }
        }
        public void SetColor(Color color)
        {
            SetPropertyColor("_Color", color);
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
        public int SortingLayerID { get => sortingGroup.sortingLayerID; set => sortingGroup.sortingLayerID = value; }

        public string SortingLayerName { get => sortingGroup.sortingLayerName; set => sortingGroup.sortingLayerName = value; }

        public int SortingOrder { get => sortingGroup.sortingOrder; set => sortingGroup.sortingOrder = value; }


        public Renderer this[int index]
        {
            get => renderers[index];
        }
        [SerializeField]
        private List<Renderer> renderers = new List<Renderer>();
        [SerializeField]
        private SortingGroup sortingGroup;
        private MaterialPropertyBlock propertyBlock;

    }
}