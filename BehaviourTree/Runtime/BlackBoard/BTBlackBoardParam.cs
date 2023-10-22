using System;

namespace SFramework.BehaviourTree.Runtime.BlackBoard
{
    public interface IBTBlackBoardParam
    {
    }
    
    [Serializable]
    public class BtIbtBlackBoardParam<T> : IBTBlackBoardParam
    {
        public string key;
        public T value;
    }
}