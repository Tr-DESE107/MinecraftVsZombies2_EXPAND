using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine.Level.Collisions
{
    public class QuadTree<T> where T : IQuadTreeNodeObject
    {
        public QuadTree(Rect size, int maxObjects = 1, int maxDepth = 5)
        {
            root = new QuadTreeNode<T>(this, null, size, 0);
            MaxObjects = maxObjects;
            MaxDepth = maxDepth;
        }
        public QuadTreeNode<T> Insert(T target)
        {
            var node = root.Insert(target);
            SetTargetNode(target, node);
            return node;
        }
        public void SetTargetNode(T target, QuadTreeNode<T> node)
        {
            targets[target] = node;
        }
        public void Remove(T target)
        {
            root.Remove(target);
            targets.Remove(target);
        }
        public void FindTargetsInRect(Rect rect, List<T> results)
        {
            root.FindTargetsInRect(rect, results);
        }
        public void GetAllTargets(List<T> results)
        {
            results.AddRange(targets.Keys);
        }
        public QuadTreeNode<T> GetRootNode()
        {
            return root;
        }
        public void Update()
        {
            refreshTargetBuffer.Clear();
            root.GetObjectsToUpdate(refreshTargetBuffer);
            foreach (var target in refreshTargetBuffer)
            {
                Remove(target);
                Insert(target);
            }
        }
        public int MaxObjects { get; }
        public int MaxDepth { get; }
        private QuadTreeNode<T> root;
        private Dictionary<T, QuadTreeNode<T>> targets = new Dictionary<T, QuadTreeNode<T>>();
        private List<T> refreshTargetBuffer = new List<T>();
    }
}
