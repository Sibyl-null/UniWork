namespace SFramework.BehaviourTree.Runtime.Node.Decorate
{
    /// <summary>
    /// 成功节点（无论子节点运行结果是什么，此节点都运行成功）
    /// </summary>
    public class SuccessNode : BaseDecorateNode
    {
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(true);
        }
    }
}