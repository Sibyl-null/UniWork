using System.Collections.Generic;
using System.Linq;

namespace SFramework.BehaviourTree.Runtime.BlackBoard
{
    public class BTBlackBoard
    {
        private readonly Dictionary<string, BTBlackBoardBaseParam> _paramDic = new Dictionary<string, BTBlackBoardBaseParam>();

        public void SetParam(string key, BTBlackBoardBaseParam baseParam)
        {
            if (_paramDic.ContainsKey(key))
                _paramDic[key] = baseParam;
            else
                _paramDic.Add(key, baseParam);
        }

        public T GetParam<T>(string key) where T : BTBlackBoardBaseParam
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

        public List<BTBlackBoardBaseParam> GetAllParamList()
        {
            return _paramDic.Values.ToList();
        }
    }
}