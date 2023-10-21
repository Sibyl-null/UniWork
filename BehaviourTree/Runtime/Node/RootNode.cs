using System;
using SFramework.BehaviourTree.Runtime.Node.Decorate;

namespace SFramework.BehaviourTree.Runtime.Node
{
    /// <summary>
    /// 根节点
    /// </summary>
    public class RootNode : BaseDecorateNode
    {
        public event Action<bool> onFinishEvent;

        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(success);
            onFinishEvent?.Invoke(success);
        }
    }
}