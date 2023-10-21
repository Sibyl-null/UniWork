using System;

namespace SFramework.BehaviourTree.Runtime.Node.Decorate
{
    /// <summary>
    /// 装饰节点基类
    /// </summary>
    public abstract class BaseDecorateNode : BaseNode
    {
        protected BaseNode _child;

        public override void AddChild(BaseNode child)
        {
            if (child == null || _child == child)
                return;
            
            if (_child != null)
                RemoveChild(_child);

            child.parentNode = this;
            _child = child;
        }

        public override void RemoveChild(BaseNode child)
        {
            if (child == null || child != _child)
                return;

            child.parentNode = null;
            _child = null;
        }

        public override void ClearChildren()
        {
            RemoveChild(_child);
        }

        public override void ForeachChildren(Action<BaseNode> action)
        {
            if (_child == null || action == null)
                return;
            
            action.Invoke(_child);
        }

        protected override void OnStart()
        {
            _child?.Start();
        }

        protected override void OnCancel()
        {
            _child?.Cancel();
        }

        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(success);
        }
    }
}