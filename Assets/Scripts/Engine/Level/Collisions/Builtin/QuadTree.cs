﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace PVZEngine.Level.Collisions
{
    public class QuadTree<T> where T : IQuadTreeNodeObject
    {
        public QuadTree(Rect size, int maxObjects = 1, int maxDepth = 5)
        {
            itemPool = new ObjectPool<QuadTreeItem<T>>(CreateQuadTreeItemFunc, actionOnRelease: a => a.Reset());
            nodePool = new ObjectPool<QuadTreeNode<T>>(CreateQuadTreeNodeFunc, actionOnRelease: a => a.Reset());

            root = CreateNode(this, null, size, 0);
            MaxObjects = maxObjects;
            MaxDepth = maxDepth;
        }
        public void Insert(T target)
        {
            var item = AddItem(target);
            var rect = target.GetCollisionRect();
            var nodeToInsert = root.EvaluateNode(rect);
            if (nodeToInsert == null)
                return;
            nodeToInsert.Insert(item);
        }
        public void Remove(T target)
        {
            var item = GetItem(target);
            if (item == null)
                return;
            item.node.Remove(item);
            RemoveItem(item);
        }
        public void FindTargetsInRect(Rect rect, List<T> results, float rewind = 0)
        {
            root.FindTargetsInRect(rect, results, rewind);
        }
        public void GetAllTargets(List<T> results)
        {
            foreach (var item in items)
            {
                results.Add(item.target);
            }
        }
        public QuadTreeNode<T> GetRootNode()
        {
            return root;
        }
        public void Update()
        {
            refreshTargetBuffer.Clear();
            refreshTargetBuffer.AddRange(items);
            foreach (var item in refreshTargetBuffer)
            {
                UpdateTarget(item);
            }
        }
        public QuadTreeNode<T> CreateNode(QuadTree<T> tree, QuadTreeNode<T> parent, Rect size, int depth = 0)
        {
            var node = nodePool.Get();
            node.Init(tree, parent, size, depth);
            return node;
        }
        public void ReleaseNode(QuadTreeNode<T> node)
        {
            nodePool.Release(node);
        }
        private void UpdateTarget(QuadTreeItem<T> item)
        {
            var rect = item.target.GetCollisionRect();
            var node = root.EvaluateNode(rect);
            item.node.MoveItemTo(item, node);
        }
        private QuadTreeItem<T> AddItem(T target)
        {
            var item = itemPool.Get();
            item.target = target;
            items.Add(item);
            return item;
        }
        private void RemoveItem(QuadTreeItem<T> item)
        {
            items.Remove(item);
            itemPool.Release(item);
        }
        private QuadTreeItem<T> GetItem(T target)
        {
            foreach (var item in items)
            {
                if (item.target.Equals(target))
                {
                    return item;
                }
            }
            return null;
        }
        private QuadTreeItem<T> CreateQuadTreeItemFunc()
        {
            return new QuadTreeItem<T>();
        }
        private QuadTreeNode<T> CreateQuadTreeNodeFunc()
        {
            return new QuadTreeNode<T>();
        }
        public int MaxObjects { get; }
        public int MaxDepth { get; }
        private QuadTreeNode<T> root;
        private List<QuadTreeItem<T>> items = new List<QuadTreeItem<T>>();
        private List<QuadTreeItem<T>> refreshTargetBuffer = new List<QuadTreeItem<T>>();
        private ObjectPool<QuadTreeItem<T>> itemPool;
        private ObjectPool<QuadTreeNode<T>> nodePool;
    }
    public class QuadTreeItem<T> where T : IQuadTreeNodeObject
    {
        public void Reset()
        {
            target = default;
            node = null;
        }

        public override string ToString()
        {
            return target.ToString();
        }

        public T target;
        public QuadTreeNode<T> node;
    }
}
