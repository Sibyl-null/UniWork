using System.Collections.Generic;
using UnityEngine;
using UniWork.BehaviourTree.Runtime.BlackBoard;
using UniWork.BehaviourTree.Runtime.Node;

namespace UniWork.BehaviourTree.Runtime.SerializeProvider
{
    public class BehaviourTreeSO : ScriptableObject, IBehaviourTreeSerializeDataProvider
    {
        public int rootId;
        
        // SerializeReference 特性可支持多态序列化
        [SerializeReference] public List<BaseNode> allNodes;
        [SerializeReference] public List<BtBlackBoardBaseParam> blackBoardParams;
        
        public void Serialize(BehaviourTree tree)
        {
            tree.SerializePreProcess();
            rootId = tree.rootId;
            allNodes = tree.allNodes;
            blackBoardParams = tree.btBlackBoard.GetAllParamList();
        }

        public BehaviourTree Deserialize()
        {
            BehaviourTree tree = new BehaviourTree()
            {
                rootId = rootId,
                allNodes = allNodes
            };
            tree.DeserializePostProcess();

            foreach (BtBlackBoardBaseParam param in blackBoardParams)
                tree.btBlackBoard.SetParam(param.key, param);
            
            return tree;
        }
    }
}