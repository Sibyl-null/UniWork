using XNode;

namespace UniWork.DialogueSystem.Runtime.Nodes
{
    public class RootNode : BaseNode
    {
        [Output] public Empty output;

        public override object GetValue(NodePort port)
        {
            return null;
        }
    }
}