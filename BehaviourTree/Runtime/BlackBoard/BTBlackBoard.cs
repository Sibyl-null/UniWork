using System.Collections.Generic;
using System.Linq;

namespace UniWork.BehaviourTree.Runtime.BlackBoard
{
    public class BtBlackBoard
    {
        private readonly Dictionary<string, BtBlackBoardBaseParam> _paramDic = new Dictionary<string, BtBlackBoardBaseParam>();

        public void SetParam(string key, BtBlackBoardBaseParam baseParam)
        {
            if (_paramDic.ContainsKey(key))
                _paramDic[key] = baseParam;
            else
                _paramDic.Add(key, baseParam);
        }

        public T GetParam<T>(string key) where T : BtBlackBoardBaseParam
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

        public List<BtBlackBoardBaseParam> GetAllParamList()
        {
            return _paramDic.Values.ToList();
        }
    }
}