using System;
using System.Collections.Generic;
using UniWork.Utility.Runtime;

namespace UniWork.RedPoint.Runtime
{
    public class RedPointNode
    {
        private readonly Dictionary<RangeString, RedPointNode> _children = new();
        private Action<RedPointNode> _onNodeRefresh;
        
        public string Name { get; private set; }
        public RedPointNode Parent { get; private set; }
        public string FullPath { get; private set; }
        public bool IsShow { get; private set; }
        
        public int ChildCount => _children.Count;
        public bool IsLeaf => _children.Count == 0;

        public RedPointNode(string name, RedPointNode parent)
        {
            Name = name;
            Parent = parent;

            if (parent == null || parent == RedPointManager.Instance.RootNode)
            {
                FullPath = name;
            }
            else
            {
                FullPath = parent.FullPath + RedPointManager.Instance.SplitChar + name;
            }
        }

        public void AddListener(Action<RedPointNode> action)
        {
            if (action != null)
                _onNodeRefresh += action;
        }

        public void RemoveListener(Action<RedPointNode> action)
        {
            if (action != null)
                _onNodeRefresh -= action;
        }

        public void SetStateIfLeaf(bool show)
        {
            if (IsLeaf == false)
            {
                DLog.Error("[RedPoint] 禁止直接设置非叶节点显隐");
                return;
            }

            IsShow = show;
            InvokeNodeRefresh();
        }
        
        private void InvokeNodeRefresh()
        {
            _onNodeRefresh?.Invoke(this);
            Parent?.InvokeNodeRefresh();
        }

        public override string ToString()
        {
            return FullPath;
        }


        // ----------------------------------------------------------------------------
        // 子节点操作
        // ----------------------------------------------------------------------------

        public RedPointNode GetOrAddChild(RangeString key)
        {
            RedPointNode node = GetChild(key) ?? AddChild(key);
            return node;
        }

        public RedPointNode GetChild(RangeString key)
        {
            return _children.TryGetValue(key, out RedPointNode node) ? node : null;
        }

        public RedPointNode AddChild(RangeString key)
        {
            if (_children.TryGetValue(key, out RedPointNode node))
            {
                DLog.Error("[RedPoint] 重复添加子节点: " + key);
                return node;
            }

            node = new RedPointNode(key.ToString(), this);
            _children.Add(key, node);
            return node;
        }

        public bool RemoveChild(RangeString key)
        {
            if (_children.ContainsKey(key) == false)
                return false;

            _children.Remove(key);
            return true;
        }
    }
}
