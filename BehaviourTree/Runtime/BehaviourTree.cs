using System.Collections.Generic;
using SFramework.BehaviourTree.Runtime.BlackBoard;
using SFramework.BehaviourTree.Runtime.Node;
using SFramework.Utility.Runtime;

namespace SFramework.BehaviourTree.Runtime
{
    public class BehaviourTree
    {
        public int rootId;
        public RootNode rootNode;
        public List<BaseNode> allNodes = new List<BaseNode>();
        public BTBlackBoard btBlackBoard = new BTBlackBoard();

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

            foreach (BaseNode node in allNodes)
                node.OnBehaviourTreeStart();                
            
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

        public BaseNode GetNode(int id)
        {
            --id;
            if (id < 0 || id >= allNodes.Count)
                return null;

            return allNodes[id];
        }
        
        // -----------------------------------------------------------------
        // 序列化支持
        // -----------------------------------------------------------------
        
        /// <summary>
        /// 序列化前的预处理
        /// </summary>
        public void SerializePreProcess()
        {
            // 设置所有节点 id，从 1 开始
            for (int i = 0; i < allNodes.Count; ++i)
                allNodes[i].id = i + 1;

            rootId = rootNode.id;
            
            // 设置所有节点的子节点 id
            foreach (BaseNode node in allNodes)
                node.RebuildChildrenId();                
        }

        /// <summary>
        /// 反序列化的后处理
        /// </summary>
        public void DeserializePostProcess()
        {
            rootNode = GetNode(rootId) as RootNode;
            
            foreach (BaseNode node in allNodes)
            {
                node.owner = this;
                node.RebuildNodeReference();
            }
        }
    }
}