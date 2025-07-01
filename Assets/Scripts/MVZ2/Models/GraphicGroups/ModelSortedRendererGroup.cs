using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Models
{
    [RequireComponent(typeof(SortingGroup))]
    public class ModelSortedRendererGroup : ModelRendererGroup
    {
        #region 公有方法
        public void CancelSortAtRoot()
        {
            foreach (var element in subSortingGroups)
            {
                if (element.ExcludedInGroup)
                    continue;
                var group = element.Group;
                group.sortAtRoot = false;
            }
        }
        #endregion

        #region 私有方法
        protected override SerializableModelGraphicGroup CreateSerializable()
        {
            var serializable = new SerializableModelRendererGroup();
            serializable.sortingLayerID = SortingLayerID;
            serializable.sortingOrder = SortingOrder;
            serializable.particles = particles.Select(e => e.ToSerializable()).ToArray();
            serializable.renderers = renderers.Select(e => e.ToSerializable()).ToArray();
            return serializable;
        }
        protected override void LoadSerializable(SerializableModelGraphicGroup serializable)
        {
            base.LoadSerializable(serializable);
            if (serializable is not SerializableModelRendererGroup spriteGroup)
                return;
            SortingLayerID = spriteGroup.sortingLayerID;
            SortingOrder = spriteGroup.sortingOrder;
        }
        #endregion

        #region 属性字段
        public int SortingLayerID
        {
            get => sortingGroup.sortingLayerID;
            set
            {
                sortingGroup.sortingLayerID = value;
                foreach (var element in subSortingGroups)
                {
                    if (element.ExcludedInGroup)
                        continue;
                    var group = element.Group;
                    if (!group.sortAtRoot)
                        continue;
                    group.sortingLayerID = value;
                }
            }
        }
        public string SortingLayerName
        {
            get => sortingGroup.sortingLayerName;
            set
            {
                sortingGroup.sortingLayerName = value;
                foreach (var element in subSortingGroups)
                {
                    if (element.ExcludedInGroup)
                        continue;
                    var group = element.Group;
                    if (!group.sortAtRoot)
                        continue;
                    group.sortingLayerName = value;
                }
            }
        }
        public int SortingOrder
        {
            get => sortingGroup.sortingOrder;
            set
            {
                sortingGroup.sortingOrder = value;
                foreach (var element in subSortingGroups)
                {
                    if (element.ExcludedInGroup)
                        continue;
                    var group = element.Group;
                    if (!group.sortAtRoot)
                        continue;
                    group.sortingOrder = value;
                }
            }
        }

        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private List<SortingGroupElement> subSortingGroups = new List<SortingGroupElement>();
        #endregion

    }
    public class SerializableModelRendererGroup : SerializableModelUnsortedRendererGroup
    {
        public int sortingLayerID;
        public int sortingOrder;
    }
}