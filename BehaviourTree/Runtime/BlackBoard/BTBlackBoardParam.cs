using System;

namespace SFramework.BehaviourTree.Runtime.BlackBoard
{
    public abstract class BTBlackBoardBaseParam
    {
        public string key;
    }
    
    [Serializable]
    public class BtBtBlackBoardBaseParam<T> : BTBlackBoardBaseParam
    {
        public T value;
    }
}