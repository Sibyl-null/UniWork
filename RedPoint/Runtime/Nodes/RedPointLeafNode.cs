using System;

namespace UniWork.RedPoint.Runtime.Nodes
{
    public class RedPointLeafNode : RedPointBaseNode
    {
        private readonly Func<bool> _judgeDisplayFunc;

        public RedPointLeafNode(string name, RedPointBaseNode parent, Func<bool> judgeDisplayFunc) : base(name, parent)
        {
            if (judgeDisplayFunc == null)
                throw new Exception("[RedPoint] 叶节点显隐判断委托不能为null");

            NodeType = RedPointType.Leaf;
            _judgeDisplayFunc = judgeDisplayFunc;
        }

        protected override void OnRefresh()
        {
            IsShow = _judgeDisplayFunc.Invoke();
        }
    }
}