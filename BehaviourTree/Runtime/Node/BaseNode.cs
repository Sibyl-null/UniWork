using System;
using SFramework.BehaviourTree.Runtime.BlackBoard;

namespace SFramework.BehaviourTree.Runtime.Node
{
    [Serializable]
    public abstract class BaseNode
    {
        public enum State
        {
            Free,       // 空闲
            Running,    // 运行中
            Success,    // 成功
            Failed      // 失败
        }

        public int id;

        [NonSerialized] public BehaviourTree owner;
        [NonSerialized] public State curState = State.Free;
        [NonSerialized] public BaseNode parentNode;

        public BTBlackBoard BlackBoard => owner.btBlackBoard;
        
        
        // -----------------------------------------------------------------
        // 节点操作
        // -----------------------------------------------------------------
        
        public abstract void AddChild(BaseNode child);
        public abstract void RemoveChild(BaseNode child);
        public abstract void ClearChildren();
        public abstract void ForeachChildren(Action<BaseNode> action);

        
        // -----------------------------------------------------------------
        // 生命周期
        // -----------------------------------------------------------------
        
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

        public virtual void OnBehaviourTreeStart()
        {
            curState = State.Free;
        }

        protected abstract void OnStart();
        protected abstract void OnCancel();
        protected abstract void OnChildFinished(BaseNode child, bool success);
        
        
        // -----------------------------------------------------------------
        // 序列化支持
        // -----------------------------------------------------------------

        public abstract void RebuildChildrenId();
        public abstract void RebuildNodeReference();
        
        // -----------------------------------------------------------------
        
        public override string ToString()
        {
            return GetType().Name;
        }
    }
}