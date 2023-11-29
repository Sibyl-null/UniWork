using UniWork.BehaviourTree.Runtime.Attribute;

namespace UniWork.BehaviourTree.Runtime.Node.Decorate
{
    [NodeInfo(name = "反转节点", desc = "对子节点运行结果取反")]
    public class InvertNode : BaseDecorateNode
    {
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(!success);
        }
    }
}