using System.Collections.Generic;

namespace SFramework.BehaviourTree.Runtime.BlackBoard
{
    public class BTBlackBoard
    {
        private readonly Dictionary<string, IBTBlackBoardParam> _paramDic = new Dictionary<string, IBTBlackBoardParam>();

        public void SetParam(string key, IBTBlackBoardParam param)
        {
            if (_paramDic.ContainsKey(key))
                _paramDic[key] = param;
            else
                _paramDic.Add(key, param);
        }

        public T GetParam<T>(string key) where T : class, IBTBlackBoardParam
        {
            if (_paramDic.ContainsKey(key) == false)
                return null;

            return _paramDic[key] as T;
        }

        public void RemoveParam(string key)
        {
            if (_paramDic.ContainsKey(key))
                _paramDic.Remove(key);
        }
    }
}