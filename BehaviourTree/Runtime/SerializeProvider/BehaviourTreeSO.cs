﻿using System.Collections.Generic;
using SFramework.BehaviourTree.Runtime.BlackBoard;
using SFramework.BehaviourTree.Runtime.Node;
using UnityEngine;

namespace SFramework.BehaviourTree.Runtime.SerializeProvider
{
    public class BehaviourTreeSO : ScriptableObject, IBehaviourTreeSerializeDataProvider
    {
        public int rootId;
        
        // SerializeReference 特性可支持多态序列化
        [SerializeReference] public List<BaseNode> allNodes;
        [SerializeReference] public List<BTBlackBoardBaseParam> blackBoardParams;
        
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

            foreach (BTBlackBoardBaseParam param in blackBoardParams)
                tree.btBlackBoard.SetParam(param.key, param);
            
            return tree;
        }
    }
}