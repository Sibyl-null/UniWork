namespace SFramework.BehaviourTree.Runtime.Node.Decorate
{
    /// <summary>
    /// 反转节点（对子节点运行结果取反）
    /// </summary>
    public class InvertNode : BaseDecorateNode
    {
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(!success);
        }
    }
}