﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVZ2.Models
{
    public class ModelGroupEntity : ModelGroupRenderer
    {
        #region 排序
        public void CancelSortAtRoot()
        {
            foreach (var element in subSortingGroups)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                var group = element.Group;
                group.sortAtRoot = false;
            }
        }
        public void SetSortingLayerID(int value)
        {
            foreach (var element in subSortingGroups)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                var group = element.Group;
                if (!group.sortAtRoot)
                    continue;
                group.sortingLayerID = value;
            }
        }
        public void SetSortingLayerName(string value)
        {
            foreach (var element in subSortingGroups)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                var group = element.Group;
                if (!group.sortAtRoot)
                    continue;
                group.sortingLayerName = value;
            }
        }
        public void SetSortingOrder(int value)
        {
            foreach (var element in subSortingGroups)
            {
                if (!element || element.ExcludedInGroup)
                    continue;
                var group = element.Group;
                if (!group.sortAtRoot)
                    continue;
                group.sortingOrder = value;
            }
        }
        #endregion

        #region 元素管理
        public override void UpdateElements()
        {
            base.UpdateElements();
            var newGroups = GetComponentsInChildren<SortingGroup>(true)
                .Where(g => g.IsDirectChild<ModelGroup>(this) && g.gameObject != gameObject)
                .Select(r =>
                {
                    var element = r.GetComponent<SortingGroupElement>();
                    if (!element)
                    {
                        element = r.gameObject.AddComponent<SortingGroupElement>();
                    }
                    return element;
                });
            subSortingGroups.ReplaceList(newGroups);
        }
        #endregion

        #region 序列化
        public override SerializableModelGroup ToSerializable()
        {
            var serializable = new SerializableModelGroupEntity();
            SaveToSerializableRenderer(serializable);
            return serializable;
        }
        #endregion

        #region 属性字段
        [SerializeField]
        private List<SortingGroupElement> subSortingGroups = new List<SortingGroupElement>();
        #endregion

    }
    public class SerializableModelGroupEntity : SerializableModelGroupRenderer
    {
    }
}