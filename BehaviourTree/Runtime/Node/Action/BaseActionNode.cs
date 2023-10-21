using System;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    /// <summary>
    /// 动作节点基类
    /// </summary>
    public abstract class BaseActionNode : BaseNode
    {
        protected sealed override void AddChild(BaseNode child)
        {
        }

        protected sealed override void RemoveChild(BaseNode child)
        {
        }

        protected sealed override void ClearChildren()
        {
        }

        protected sealed override void ForeachChildren(Action<BaseNode> action)
        {
        }

        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
        }
    }
}