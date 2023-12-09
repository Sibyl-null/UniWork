using System;
using System.Collections.Generic;
using UniWork.Utility.Runtime;

namespace UniWork.RedPoint.Runtime
{
    public class RedPointNode
    {
        private readonly Dictionary<RangeString, RedPointNode> _children = new Dictionary<RangeString, RedPointNode>();
        private Action<int> _onValueChanged;
        
        public string Name { get; private set; }
        public int Value { get; private set; }      // 叶节点值单独计数，非叶节点值为子节点值总和
        public RedPointNode Parent { get; private set; }
        public string FullPath { get; private set; }
        public bool IsLeaf => _children.Count == 0;

        public RedPointNode(string name, RedPointNode parent)
        {
            Name = name;
            Value = 0;
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

        public void AddListener(Action<int> action)
        {
            if (action != null)
                _onValueChanged += action;
        }

        public void RemoveListener(Action<int> action)
        {
            if (action != null)
                _onValueChanged -= action;
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
        
        
        // ----------------------------------------------------------------------------
        // 节点值更改
        // ----------------------------------------------------------------------------

        /**
         * 改变节点值，只能在叶节点上调用
         */
        public void ChangeValueBySelf(int value)
        {
            if (!IsLeaf)
            {
                DLog.Error($"[RedPoint] {nameof(ChangeValueBySelf)} 方法不能在非叶节点上调用");
                return;
            }

            ChangeValueInternal(value);
        }

        /**
         * 根据子节点刷新节点值，只能在非叶节点上调用
         */
        public void ChangeValueByChild()
        {
            if (IsLeaf)
            {
                DLog.Error($"[RedPoint] {nameof(ChangeValueByChild)} 方法不能在叶节点上调用");
                return;
            }

            int sum = 0;
            foreach (RedPointNode node in _children.Values)
                sum += node.Value;
            
            ChangeValueInternal(sum);
        }
        
        private void ChangeValueInternal(int newValue)
        {
            if (Value == newValue)
                return;

            Value = newValue;
            _onValueChanged?.Invoke(newValue);
        }
    }
}
