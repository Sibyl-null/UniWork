using System;
using System.Collections.Generic;
using System.Text;
using UniWork.RedPoint.Runtime.Nodes;
using UniWork.Utility.Runtime;

// Note: 分为两种节点, 过程节点和叶节点. 过程节点 - 子节点显示数量改变事件. 叶节点 - 是否显隐事件.
// 外部只能添加移除叶节点。提供刷新叶节点方法 -> 调用叶节点事件 
// 事先定义一堆叶节点路径，初始化时添加。即整个红点树在初始化时就构建完成，节点不论显隐都在树中
// UI: 根据路径找到节点，注册事件。 调用叶节点刷新。

namespace UniWork.RedPoint.Runtime
{
    public struct LeafNodeDefine
    {
        public readonly string Path;
        public readonly Func<bool> Func;

        public LeafNodeDefine(string path, Func<bool> func)
        {
            Path = path;
            Func = func;
        }
    }
    
    public static class RedPointTree
    {
        // FullPath -> RedPointBaseNode
        private static readonly Dictionary<string, RedPointBaseNode> _nodes = new();

        public static char SplitChar => '|';
        public static StringBuilder CachedSb { get; } = new();
        private static RedPointRouteNode RootNode { get; set; }

        public static void Create(List<LeafNodeDefine> leafNodeDefines)
        {
            RootNode = new RedPointRouteNode("RootNode", null);
            foreach (LeafNodeDefine define in leafNodeDefines)
                AddLeafNode(define.Path, define.Func);
        }

        public static void Release()
        {
            _nodes.Clear();
            CachedSb.Clear();
            RootNode = null;
        }

        public static void RefreshNode(string path)
        {
            RedPointBaseNode node = GetNode(path);
            if (node == null)
            {
                DLog.Error("[RedPoint] 该路径不存在节点: " + path);
                return;
            }
            
            node.Refresh();
        }
        
        public static RedPointBaseNode GetNode(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                DLog.Warning("[RedPoint] 路径为空");
                return null;
            }

            return _nodes.TryGetValue(path, out RedPointBaseNode node) ? node : null;
        }

        public static RedPointLeafNode AddLeafNode(string path, Func<bool> judgeDisplayFunc)
        {
            if (GetNode(path) != null)
                throw new Exception("[RedPoint] 重复添加节点: " + path);

            RedPointRouteNode routeNode = RootNode;
            int startIndex = 0;

            for (int i = 0; i < path.Length; ++i)
            {
                if (path[i] != SplitChar)
                    continue;

                routeNode = routeNode.GetOrAddRouteChild(new RangeString(path, startIndex, i - 1));
                startIndex = i + 1;

                string subPath = path.Substring(0, i);
                if (_nodes.ContainsKey(subPath) == false)
                    _nodes.Add(subPath, routeNode);
            }

            // 最后一个节点是叶节点
            RedPointLeafNode leafNode =
                routeNode.GetOrAddLeafChild(new RangeString(path, startIndex, path.Length - 1), judgeDisplayFunc);
            leafNode.Refresh();

            _nodes.Add(path, leafNode);
            return leafNode;
        }

        public static void RemoveLeafNode(string path)
        {
            if (GetNode(path) == null)
                throw new Exception("[RedPoint] 不存在该节点: " + path);

            RedPointBaseNode node = _nodes[path];
            if (node.NodeType != RedPointType.Leaf)
                throw new Exception("[RedPoint] 不允许移除非叶节点: " + path);

            RedPointRouteNode parent = (RedPointRouteNode)node.Parent;
            parent.RemoveChild(new RangeString(node.Name, 0, node.Name.Length - 1));
            parent.Refresh();
            
            _nodes.Remove(path);
        }
    }
}