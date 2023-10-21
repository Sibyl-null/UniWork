using System;

namespace SFramework.BehaviourTree.Runtime.Node
{
    public abstract class BaseNode
    {
        public enum State
        {
            Free,       // 空闲
            Running,    // 运行中
            Success,    // 成功
            Failed      // 失败
        }

        public State curState = State.Free;
        public BaseNode parentNode;

        protected abstract void AddChild(BaseNode child);
        protected abstract void RemoveChild(BaseNode child);
        protected abstract void ClearChildren();
        protected abstract void ForeachChildren(Action<BaseNode> action);

        public void Start()
        {
            if (curState == State.Running)
                return;

            curState = State.Running;
            OnStart();
        }

        public void Cancel()
        {
            if (curState != State.Running)
                return;

            curState = State.Free;
            OnCancel();
        }

        public void Finish(bool success)
        {
            curState = success ? State.Success : State.Failed;
            // 通知父节点自身运行结果
            parentNode?.OnChildFinished(this, success);
        }

        protected abstract void OnStart();
        protected abstract void OnCancel();
        protected abstract void OnChildFinished(BaseNode child, bool success);

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}