using System;
using System.Collections.Generic;
using UniWork.Utility.Runtime;

namespace UniWork.RedPoint.Runtime.Nodes
{
    public class RedPointRouteNode : RedPointBaseNode
    {
        private readonly Dictionary<RangeString, RedPointBaseNode> _children = new();
        
        public int ChildShowCount { get; private set; }

        public RedPointRouteNode(string name, RedPointBaseNode parent) : base(name, parent)
        {
            NodeType = RedPointType.Route;
        }

        protected override void OnRefresh()
        {
            ChildShowCount = 0;
            foreach (RedPointBaseNode childNode in _children.Values)
            {
                if (childNode.IsShow)
                    ++ChildShowCount;
            }

            IsShow = ChildShowCount != 0;
        }


        // ----------------------------------------------------------------------------
        // 子节点操作
        // ----------------------------------------------------------------------------

        public RedPointRouteNode GetOrAddRouteChild(RangeString key)
        {
            RedPointBaseNode node = GetChild(key) ?? AddRouteChild(key);
            return (RedPointRouteNode)node;
        }
        
        public RedPointLeafNode GetOrAddLeafChild(RangeString key, Func<bool> judgeDisplayFunc)
        {
            RedPointBaseNode node = GetChild(key) ?? AddLeafChild(key, judgeDisplayFunc);
            return (RedPointLeafNode)node;
        }

        public RedPointBaseNode AddRouteChild(RangeString key)
        {
            if (_children.TryGetValue(key, out RedPointBaseNode node))
            {
                DLog.Error("[RedPoint] 重复添加子节点: " + key);
                return node;
            }

            node = new RedPointRouteNode(key.ToString(), this);
            _children.Add(key, node);
            return node;
        }

        public RedPointBaseNode AddLeafChild(RangeString key, Func<bool> judgeDisplayFunc)
        {
            if (_children.TryGetValue(key, out RedPointBaseNode node))
            {
                DLog.Error("[RedPoint] 重复添加子节点: " + key);
                return node;
            }

            node = new RedPointLeafNode(key.ToString(), this, judgeDisplayFunc);
            _children.Add(key, node);
            return node;
        }
        
        public RedPointBaseNode GetChild(RangeString key)
        {
            return _children.TryGetValue(key, out RedPointBaseNode node) ? node : null;
        }

        public void RemoveChild(RangeString key)
        {
            if (_children.ContainsKey(key) == false)
            {
                DLog.Error("[RedPoint] 不存在该节点: " + key);
                return;
            }

            _children.Remove(key);
        }
    }
}