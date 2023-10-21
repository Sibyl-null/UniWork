using System;
using System.Collections.Generic;

namespace SFramework.BehaviourTree.Runtime.Node.Composite
{
    /// <summary>
    /// 装饰节点基类
    /// </summary>
    public abstract class BaseCompositeNode : BaseNode
    {
        protected readonly List<BaseNode> _children = new List<BaseNode>();

        protected override void AddChild(BaseNode child)
        {
            if (child == null)
                return;

            child.parentNode = this;
            _children.Add(child);
        }

        protected override void RemoveChild(BaseNode child)
        {
            if (child == null)
                return;

            child.parentNode = null;
            _children.Remove(child);
        }

        protected override void ClearChildren()
        {
            for (int i = _children.Count - 1; i >= 0; --i)
                RemoveChild(_children[i]);
        }

        protected override void ForeachChildren(Action<BaseNode> action)
        {
            if (action == null)
                return;
            
            foreach (BaseNode child in _children)
                action.Invoke(child);
        }
    }
}