using SFramework.BehaviourTree.Runtime.Attribute;

namespace SFramework.BehaviourTree.Runtime.Node.Composite
{
    [NodeInfo(name = "序列节点", desc = "依次执行子节点，直到有子节点运行失败，则此节点运行失败，否则成功")]
    public class SequenceNode : BaseCompositeNode
    {
        private int _curChildIndex = -1;
        
        protected override void OnStart()
        {
            _curChildIndex = -1;
            StartNextChild();
        }

        protected override void OnCancel()
        {
            if (_curChildIndex >= 0 && _curChildIndex < _children.Count)
                _children[_curChildIndex].Cancel();
        }

        protected override void OnChildFinished(BaseNode child, bool success)
        {
            if (success)
                StartNextChild();
            else
                Finish(false);
        }

        private void StartNextChild()
        {
            ++_curChildIndex;
            if (_curChildIndex >= _children.Count)
            {
                Finish(true);
                return;
            }
            
            _children[_curChildIndex].Start();
        }
    }
}