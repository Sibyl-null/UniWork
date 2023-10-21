using SFramework.BehaviourTree.Runtime.Node;
using SFramework.Utility.Runtime;

namespace SFramework.BehaviourTree.Runtime
{
    public class BehaviourTree
    {
        public RootNode rootNode;

        public static BehaviourTree Create()
        {
            return new BehaviourTree();
        }

        public void Start()
        {
            if (rootNode == null)
            {
                DLog.Error("[BehaviourTree] RootNode is Null");
                return;
            }
            
            if (rootNode.curState == BaseNode.State.Running)
                return;
            
            rootNode.Start();
        }

        public void Cancel()
        {
            if (rootNode == null)
            {
                DLog.Error("[BehaviourTree] RootNode is Null");
                return;
            }
            
            if (rootNode.curState != BaseNode.State.Running)
                return;
            
            rootNode.Cancel();
        }

        public void ReStart()
        {
            Cancel();
            rootNode.Start();
        }
    }
}