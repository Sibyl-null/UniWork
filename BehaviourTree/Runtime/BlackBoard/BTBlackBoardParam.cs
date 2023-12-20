using System;

namespace UniWork.BehaviourTree.Runtime.BlackBoard
{
    [Serializable]
    public abstract class BtBlackBoardBaseParam
    {
        public string key;
    }
    
    [Serializable]
    public class BtBtBlackBoardBaseParam<T> : BtBlackBoardBaseParam
    {
        public T value;
    }
}