using UniWork.BehaviourTree.Runtime.Attribute;

namespace UniWork.BehaviourTree.Runtime.Node.Composite
{
    [NodeInfo(name = "选择节点", desc = "依次执行子节点，直到有子节点运行成功，则此节点运行成功，否则失败")]
    public class SelectorNode : BaseCompositeNode
    {
        private int _curChildIndex = -1;
        
        protected override void OnStart()
        {
            _curChildIndex = -1;
            StartNextNode();
        }

        protected override void OnCancel()
        {
            if (_curChildIndex >= 0 && _curChildIndex < _children.Count)
                _children[_curChildIndex].Cancel();
        }

        protected override void OnChildFinished(BaseNode child, bool success)
        {
            if (success)
                Finish(true);
            else
                StartNextNode();
        }

        private void StartNextNode()
        {
            ++_curChildIndex;
            if (_curChildIndex >= _children.Count)
            {
                Finish(false);
                return;
            }
            
            _children[_curChildIndex].Start();
        }
    }
}