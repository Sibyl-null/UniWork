using System;
using System.Collections.Generic;

namespace SFramework.BehaviourTree.Runtime.Node.Composite
{
    /// <summary>
    /// 复合节点基类
    /// </summary>
    public abstract class BaseCompositeNode : BaseNode
    {
        protected readonly List<BaseNode> _children = new List<BaseNode>();

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
    }
}