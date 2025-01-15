using System.Collections.Generic;
using UnityEngine;

namespace PVZEngine.Level.Collisions
{
    public class QuadTreeNode<T> where T : IQuadTreeNodeObject
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public QuadTreeNode(QuadTree<T> tree, QuadTreeNode<T> parent, Rect size, int depth = 0)
        {
            Tree = tree;
            Parent = parent;
            bounds = size;
            looseBounds = bounds;
            looseBounds.position -= looseBounds.size;
            looseBounds.size *= 2;
            this.depth = depth;
        }
        public Rect GetRect()
        {
            return bounds;
        }
        public int GetChildCount()
        {
            return children.Count;
        }
        public QuadTreeNode<T> GetChild(int i)
        {
            return children[i];
        }
        public bool ContainsStrict(Rect rect)
        {
            return rect.xMin > bounds.xMin && rect.xMax < bounds.xMax && rect.yMin > bounds.yMin && rect.yMax < bounds.yMax;
        }
        public bool ContainsLoose(Rect rect)
        {
            return rect.xMin > looseBounds.xMin && rect.xMax < looseBounds.xMax && rect.yMin > looseBounds.yMin && rect.yMax < looseBounds.yMax;
        }
        /// <summary>
        /// 插入一个新的Rect
        /// </summary>
        public QuadTreeNode<T> Insert(T target)
        {
            //获取到这个物体的Rect
            Rect rect = target.GetCollisionRect();

            //判断是否创建子节点
            if (children.Count <= 0 && (totalTargetCount >= Tree.MaxObjects && depth < Tree.MaxDepth))
            {
                // 分裂，并将完全位于子节点中的物体分给子节点。
                CreateChildren();
                //填充对象到新创建的子节点中
                SpreadObjectsToChildren();
            }
            // 如果有子节点
            if (children.Count > 0)
            {
                //尽可能地分到子节点
                foreach (var child in children)
                {
                    if (child.ContainsStrict(rect))
                    {
                        // 完全位于子节点中，加入到子节点中。
                        totalTargetCount++;
                        return child.Insert(target);
                    }
                }
            }
            // 没有子节点，或者踩线了
            targets.Add(target);
            totalTargetCount++;
            return this;
        }
        public bool Remove(T target)
        {
            //父节点坍缩
            //遍历移除
            if (targets.Remove(target))
            {
                LoseObject();
                return true;
            }

            //如果当前节点没有匹配对象，则遍历子节点寻找需要移除的对象
            if (children.Count > 0)
            {
                foreach (var child in children)
                {
                    if (child.Remove(target))
                    {
                        LoseObject();
                        return true;
                    }
                }
            }
            return false;
        }
        public void FindTargetsInRect(Rect rect, List<T> results)
        {
            if (!rect.Overlaps(looseBounds))
                return;

            //查找匹配对象
            foreach (var target in targets)
            {
                if (rect.Overlaps(target.GetCollisionRect()))
                {
                    results.Add(target);
                }
            }

            //遍历子节点
            foreach (var child in children)
            {
                child.FindTargetsInRect(rect, results);
            }
        }
        public void GetObjectsToUpdate(List<T> results)
        {
            foreach (var target in targets)
            {
                var rect = target.GetCollisionRect();
                if (!bounds.Overlaps(rect))
                {
                    // 完全离开所属区域
                    results.Add(target);
                }
            }
            foreach (var child in children)
            {
                child.GetObjectsToUpdate(results);
            }
        }
        /// <summary>
        /// 分割四叉树，为这个结点计算出四个子结点
        /// </summary>
        private void CreateChildren()
        {
            //这部分是计算子结点的x,y,w,h
            float halfWidth = bounds.width / 2;
            float halfHeight = bounds.height / 2;
            float x = bounds.x;
            float y = bounds.y;
            children.Add(new QuadTreeNode<T>(Tree, this, new Rect(x, y + halfHeight, halfWidth, halfHeight), depth + 1));
            children.Add(new QuadTreeNode<T>(Tree, this, new Rect(x + halfWidth, y + halfHeight, halfWidth, halfHeight), depth + 1));
            children.Add(new QuadTreeNode<T>(Tree, this, new Rect(x, y, halfWidth, halfHeight), depth + 1));
            children.Add(new QuadTreeNode<T>(Tree, this, new Rect(x + halfWidth, y, halfWidth, halfHeight), depth + 1));
        }
        private void RemoveChildren()
        {
            children.Clear();
        }
        private void SpreadObjectsToChildren()
        {
            for (var i = targets.Count - 1; i >= 0; i--)
            {
                var movingTarget = targets[i];
                var movingRect = movingTarget.GetCollisionRect();
                //遍历子节点
                foreach (var child in children)
                {
                    //对象满足子节点条件的，插入到子节点中
                    if (child.ContainsStrict(movingRect))
                    {
                        var newNode = child.Insert(movingTarget);
                        targets.RemoveAt(i);
                        Tree.SetTargetNode(movingTarget, newNode);
                        break;
                    }
                }
            }
        }
        private void RecycleObjectsFromChildren()
        {
            //遍历子节点
            foreach (var child in children)
            {
                for (var i = child.targets.Count - 1; i >= 0; i--)
                {
                    var recycling = child.targets[i];
                    targets.Add(recycling);
                    Tree.SetTargetNode(recycling, this);
                }
            }
        }
        private void LoseObject()
        {
            totalTargetCount--;
            if (totalTargetCount <= Tree.MaxObjects)
            {
                RecycleObjectsFromChildren();
                RemoveChildren();
            }
        }
        public QuadTree<T> Tree { get; }
        public QuadTreeNode<T> Parent { get; }
        //rect范围
        private Rect bounds;
        private Rect looseBounds;
        //当前层级内的对象
        private List<T> targets = new List<T>();
        private int totalTargetCount;
        //子节点
        private readonly List<QuadTreeNode<T>> children = new List<QuadTreeNode<T>>();
        //当前层级
        private readonly int depth;
    }
    public interface IQuadTreeNodeObject
    {
        Rect GetCollisionRect();
    }
}
