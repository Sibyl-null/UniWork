﻿using System;
using System.Collections.Generic;
using System.Text;
using UniWork.Utility.Runtime;

namespace UniWork.RedPoint.Runtime
{
    public class RedPointManager
    {
        public static RedPointManager Instance { get; } = new RedPointManager();

        // FullPath -> RedPointNode
        private readonly Dictionary<string, RedPointNode> _nodes = new Dictionary<string, RedPointNode>();
        private HashSet<RedPointNode> _dirtyNodeSet = new HashSet<RedPointNode>();
        private List<RedPointNode> _tempDirtyNodes = new List<RedPointNode>();

        public char SplitChar { get; } = '/';
        public StringBuilder CachedSb { get; } = new StringBuilder();
        public RedPointNode RootNode { get; } = new RedPointNode("RootNode", null);

        public void Tick()
        {
            if (_dirtyNodeSet.Count == 0)
                return;
            
            _tempDirtyNodes.Clear();
            foreach (RedPointNode node in _dirtyNodeSet)
                _tempDirtyNodes.Add(node);
            
            _dirtyNodeSet.Clear();

            foreach (RedPointNode node in _tempDirtyNodes)
                node.ChangeValueByChild();
        }

        public void MarkDirtyNode(RedPointNode node)
        {
            if (node == null || node == RootNode)
                return;

            _dirtyNodeSet.Add(node);
        }
        
        public RedPointNode GetNode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                DLog.Warning("[RedPoint] 路径为空");
                return null;
            }

            return _nodes.TryGetValue(path, out RedPointNode node) ? node : null;
        }

        public RedPointNode AddNode(string path)
        {
            RedPointNode node = GetNode(path);
            if (node != null)
            {
                DLog.Warning("[RedPoint] RedPoint 已存在: " + path);
                return node;
            }

            node = RootNode;
            int startIndex = 0;

            for (int i = 0; i < path.Length; ++i)
            {
                if (path[i] != SplitChar)
                    continue;

                node = node.GetOrAddChild(new RangeString(path, startIndex, i - 1));
                startIndex = i + 1;
            }

            node = node.GetOrAddChild(new RangeString(path, startIndex, path.Length - 1));
            _nodes.Add(path, node);
            return node;
        }

        public bool RemoveNode(string path)
        {
            if (_nodes.ContainsKey(path) == false)
                return false;

            RedPointNode node = _nodes[path];
            _nodes.Remove(path);
            return node.Parent.RemoveChild(new RangeString(node.Name, 0, node.Name.Length - 1));
        }
        
        public void AddNodeListener(string path, Action<int> valueChangedAction)
        {
            RedPointNode node = GetNode(path);
            node.AddListener(valueChangedAction);
        }

        public void RemoveNodeListener(string path, Action<int> valueChangedAction)
        {
            RedPointNode node = GetNode(path);
            node.RemoveListener(valueChangedAction);
        }
    }
}