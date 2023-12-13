using System;
using System.Collections.Generic;
using UnityEngine;
using UniWork.BehaviourTree.Runtime.BlackBoard;
using UniWork.BehaviourTree.Runtime.Node;

namespace UniWork.BehaviourTree.Runtime.SerializeProvider
{
    [Serializable]
    public class BehaviourTreeSO : ScriptableObject, IBehaviourTreeSerializeDataProvider
    {
        public int rootId;
        
        // SerializeReference 特性可支持多态序列化
        [SerializeReference] public List<BaseNode> allNodes;
        [SerializeReference] public List<BtBlackBoardBaseParam> blackBoardParams;
        
        public void Serialize(BehaviourTree tree)
        {
            tree.SerializePreProcess();
            allNodes = tree.allNodes;
            blackBoardParams = tree.btBlackBoard.GetAllParamList();
        }

        public BehaviourTree Deserialize()
        {
            BehaviourTree tree = new BehaviourTree()
            {
                allNodes = allNodes
            };
            
            foreach (BtBlackBoardBaseParam param in blackBoardParams)
                tree.btBlackBoard.SetParam(param.key, param);
            
            tree.DeserializePostProcess();
            return tree;
        }
    }
}