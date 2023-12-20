using System;

namespace UniWork.RedPoint.Runtime.Nodes
{
    public enum RedPointType
    {
        Leaf, Route
    }
    
    public abstract class RedPointBaseNode
    {
        public RedPointType NodeType { get; protected set; }
        public string Name { get; private set; }
        public RedPointBaseNode Parent { get; private set; }
        public bool IsShow { get; protected set; }

        private Action<RedPointBaseNode> _refreshCallback;

        protected RedPointBaseNode(string name, RedPointBaseNode parent)
        {
            Name = name;
            Parent = parent;
        }

        public void Refresh()
        {
            OnRefresh();
            _refreshCallback?.Invoke(this);
            Parent?.Refresh();
        }

        public void RegisterRefreshCallback(Action<RedPointBaseNode> callback)
        {
            if (callback == null)
                return;

            _refreshCallback += callback;
        }

        public void UnRegisterRefreshCallback(Action<RedPointBaseNode> callback)
        {
            if (callback == null)
                return;

            _refreshCallback -= callback;
        }

        protected abstract void OnRefresh();
    }
}