using System;
using System.Collections.Generic;
using SFramework.BehaviourTree.Runtime.Attribute;

namespace SFramework.BehaviourTree.Runtime.Node.Composite
{
    /// <summary>
    /// 复合节点基类
    /// </summary>
    [Serializable]
    [ChildCapacityInfo(capacity = ChildCapacity.Multi)]
    public abstract class BaseCompositeNode : BaseNode
    {
        public List<int> childrenIds = new List<int>();
        
        [NonSerialized] protected readonly List<BaseNode> _children = new List<BaseNode>();

        public override void AddChild(BaseNode child)
        {
            if (child == null)
                return;

            child.parentNode = this;
            _children.Add(child);
        }

        public override void RemoveChild(BaseNode child)
        {
            if (child == null || !_children.Contains(child))
                return;

            child.parentNode = null;
            _children.Remove(child);
        }

        public override void ClearChildren()
        {
            for (int i = _children.Count - 1; i >= 0; --i)
                RemoveChild(_children[i]);
        }

        public override void ForeachChildren(Action<BaseNode> action)
        {
            if (action == null || _children.Count == 0)
                return;
            
            foreach (BaseNode child in _children)
                action.Invoke(child);
        }
        
        // ----------------------------------------------------------------

        public override void RebuildChildrenId()
        {
            childrenIds.Clear();
            foreach (BaseNode node in _children)
                childrenIds.Add(node.id);                
        }

        public override void RebuildNodeReference()
        {
            _children.Clear();
            foreach (int childId in childrenIds)
            {
                if (childId != 0)
                    AddChild(owner.GetNode(childId));
            }
        }
    }
}