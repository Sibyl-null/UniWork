using System;

namespace UniWork.BehaviourTree.Runtime.BlackBoard
{
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