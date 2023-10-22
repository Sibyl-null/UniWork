using System;
using SFramework.BehaviourTree.Runtime.Attribute;
using SFramework.BehaviourTree.Runtime.Node.Decorate;

namespace SFramework.BehaviourTree.Runtime.Node
{
    [NodeInfo(name = "根节点")]
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