using System.Collections.Generic;
using SFramework.BehaviourTree.Runtime.Node;
using UnityEngine;

namespace SFramework.BehaviourTree.Runtime.SerializeProvider
{
    public class BehaviourTreeSO : ScriptableObject, IBehaviourTreeSerializeDataProvider
    {
        public int rootId;
        // SerializeReference 特性可支持多态序列化
        [SerializeReference] public List<BaseNode> allNodes;
        
        public void Serialize(BehaviourTree tree)
        {
            tree.SerializePreProcess();
            rootId = tree.rootId;
            allNodes = tree.allNodes;
        }

        public BehaviourTree Deserialize()
        {
            BehaviourTree tree = new BehaviourTree()
            {
                rootId = rootId,
                allNodes = allNodes
            };
            tree.DeserializePostProcess();
            return tree;
        }
    }
}