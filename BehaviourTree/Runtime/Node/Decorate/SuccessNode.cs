using UniWork.BehaviourTree.Runtime.Attribute;

namespace UniWork.BehaviourTree.Runtime.Node.Decorate
{
    [NodeInfo(name = "成功节点", desc = "无论子节点运行结果是什么，此节点都运行成功")]
    public class SuccessNode : BaseDecorateNode
    {
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(true);
        }
    }
}