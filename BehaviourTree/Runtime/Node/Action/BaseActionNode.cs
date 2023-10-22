using System;
using SFramework.BehaviourTree.Runtime.Attribute;

namespace SFramework.BehaviourTree.Runtime.Node.Action
{
    /// <summary>
    /// 动作节点基类
    /// </summary>
    [ChildCapacityInfo(capacity = ChildCapacity.None)]
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
        
        // ----------------------------------------------------------------

        protected sealed override void OnChildFinished(BaseNode child, bool success)
        {
        }
        
        // ----------------------------------------------------------------

        public sealed override void RebuildChildrenId()
        {
        }

        public sealed override void RebuildNodeReference()
        {
        }
    }
}