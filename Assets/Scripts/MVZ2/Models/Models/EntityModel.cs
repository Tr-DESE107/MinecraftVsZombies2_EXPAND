﻿using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Models
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SortingGroup))]
    public sealed class EntityModel : Model
    {
        public void CancelSortAtRoot()
        {
            RendererGroup.CancelSortAtRoot();
        }
        public void SetLightVisible(bool visible)
        {
            bone.SetLightVisible(visible);
        }
        public void SetLightColor(Color color)
        {
            bone.SetLightColor(color);
        }
        public void SetLightRange(Vector2 range)
        {
            bone.SetLightRange(range);
        }
        public void SetColliderActive(bool active)
        {
            if (modelCollider)
            {
                modelCollider.enabled = active;
            }
        }
        public override void UpdateElements()
        {
            base.UpdateElements();
            var newCollider = GetComponentsInChildren<Collider2D>(true)
                .Where(g => g.IsDirectChild<Model>(this)).FirstOrDefault();
            if (newCollider != modelCollider)
            {
                modelCollider = newCollider;
            }
            var newSortingGroup = GetComponent<SortingGroup>();
            if (newSortingGroup != sortingGroup)
            {
                sortingGroup = newSortingGroup;
            }
            var newGroup = GetComponent<ModelGroupEntity>();
            if (newGroup != group)
            {
                group = newGroup;
            }
            var newBone = GetComponentsInChildren<ModelBone>(true)
                .Where(g => g.IsDirectChild<Model>(this) && g.gameObject != gameObject).FirstOrDefault();
            if (newBone != bone)
            {
                bone = newBone;
            }
        }
        protected override SerializableModelData CreateSerializable()
        {
            var serializable = new SerializableSpriteModelData();
            return serializable;
        }
        public int SortingLayerID
        {
            get => sortingGroup.sortingLayerID;
            set
            {
                sortingGroup.sortingLayerID = value;
                RendererGroup.SetSortingLayerID(value);
            }
        }
        public string SortingLayerName
        {
            get => sortingGroup.sortingLayerName;
            set
            {
                sortingGroup.sortingLayerName = value;
                RendererGroup.SetSortingLayerName(value);
            }
        }
        public int SortingOrder
        {
            get => sortingGroup.sortingOrder;
            set
            {
                sortingGroup.sortingOrder = value;
                RendererGroup.SetSortingOrder(value);
            }
        }
        public override ModelGroup GraphicGroup => RendererGroup;
        public Collider2D Collider => modelCollider;
        public ModelGroupEntity RendererGroup => group;
        [Header("Entity")]
        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private Collider2D modelCollider;
        [SerializeField]
        private ModelGroupEntity group;
        [SerializeField]
        private ModelBone bone;
    }
    public class SerializableSpriteModelData : SerializableModelData
    {
    }
}