namespace SFramework.BehaviourTree.Runtime.Node.Decorate
{
    /// <summary>
    /// 失败节点（无论子节点运行结果是什么，此节点都运行失败）
    /// </summary>
    public class FailureNode : BaseDecorateNode
    {
        protected override void OnChildFinished(BaseNode child, bool success)
        {
            Finish(false);
        }
    }
}