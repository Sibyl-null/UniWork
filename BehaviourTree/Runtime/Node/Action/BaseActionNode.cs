using System;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    /// <summary>
    /// 动作节点基类
    /// </summary>
    public abstract class BaseActionNode : BaseNode
    {
        public sealed override void AddChild(BaseNode child)
        {
        }

        public sealed override void RemoveChild(BaseNode child)
        {
        }

        public sealed override void ClearChildren()
        {
        }

        public sealed override void ForeachChildren(Action<BaseNode> action)
        {
        }

        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
        }
    }
}