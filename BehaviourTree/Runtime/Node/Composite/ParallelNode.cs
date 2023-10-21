namespace SFramework.BehaviourTree.Runtime.Node.Composite
{
    /// <summary>
    /// 并行节点（同时运行所有节点，根据设置的并行条件决定此节点如何判断成功/失败）
    /// </summary>
    public class ParallelNode : BaseCompositeNode
    {
        public enum ParallelCondition
        {
            FirstSuccess,   // 任意一个子节点成功，则此节点成功，否则当所有子节点失败时，此节点失败
            FirstFailure    // 任意一个子节点失败，则此节点失败，否则当所有子节点成功时，此节点成功
        }

        private ParallelCondition _condition = ParallelCondition.FirstSuccess;
        private int _successCount = 0;
        private int _failureCount = 0;
        
        protected override void OnStart()
        {
            _successCount = 0;
            _failureCount = 0;
            
            foreach (BaseNode child in _children)
                child.Start();
        }

        protected override void OnCancel()
        {
            foreach (BaseNode child in _children)
                child.Cancel();
        }

        protected override void OnChildFinished(BaseNode child, bool success)
        {
            _successCount += success ? 1 : 0;
            _failureCount += success ? 0 : 1;

            switch (_condition)
            {
                case ParallelCondition.FirstSuccess:
                    if (_successCount > 0)
                    {
                        Cancel();
                        Finish(true);
                    }
                    else if (_failureCount == _children.Count)
                    {
                        Cancel();
                        Finish(false);
                    }
                    break;
                case ParallelCondition.FirstFailure:
                    if (_failureCount > 0)
                    {
                        Cancel();
                        Finish(false);
                    }
                    else if (_successCount == _children.Count)
                    {
                        Cancel();
                        Finish(true);
                    }
                    break;
            }
        }
    }
}