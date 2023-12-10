using System;
using System.Collections.Generic;
using System.Text;
using UniWork.Utility.Runtime;

// Note: 分为两种节点, 过程节点和叶节点. 过程节点 - 子节点显示数量改变事件. 叶节点 - 是否显隐事件.
// 外部只能添加移除叶节点。提供刷新叶节点方法 -> 调用叶节点事件 
// 事先定义一堆叶节点路径，初始化时添加。即整个红点树在初始化时就构建完成，节点不论显隐都在树中
// UI: 根据路径找到节点，注册事件。 调用叶节点刷新。

namespace UniWork.RedPoint.Runtime
{
    public class RedPointManager
    {
        public static RedPointManager Instance { get; } = new RedPointManager();

        // FullPath -> RedPointNode
        private readonly Dictionary<string, RedPointNode> _nodes = new Dictionary<string, RedPointNode>();

        public char SplitChar { get; } = '|';
        public StringBuilder CachedSb { get; } = new StringBuilder();
        public RedPointNode RootNode { get; } = new RedPointNode("RootNode", null);
        
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
                return node;

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

        public void SetLeafNodeState(string path, bool show)
        {
            RedPointNode node = GetNode(path);
            if (node == null)
                return;
            
            node.SetStateIfLeaf(show);
        }
        
        public void AddNodeListener(string path, Action<RedPointNode> nodeRefreshAction)
        {
            RedPointNode node = GetNode(path);
            node.AddListener(nodeRefreshAction);
        }

        public void RemoveNodeListener(string path, Action<RedPointNode> nodeRefreshAction)
        {
            RedPointNode node = GetNode(path);
            node.RemoveListener(nodeRefreshAction);
        }
    }
}