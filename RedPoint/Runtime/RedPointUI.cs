using System;
using UnityEngine;
using UnityEngine.UI;
using UniWork.RedPoint.Runtime.Nodes;

namespace UniWork.RedPoint.Runtime
{
    [RequireComponent(typeof(Image))]
    public class RedPointUI : MonoBehaviour
    {
        public string path;

        private Image _image;
        private RedPointBaseNode _node;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _node = RedPointManager.Instance.GetNode(path);
            
            RegisterRefresh(SetIconActive);
            Refresh();
        }

        private void OnDestroy()
        {
            UnRegisterRefresh(SetIconActive);
        }

        private void SetIconActive(RedPointBaseNode node)
        {
            _image.gameObject.SetActive(node.IsShow);
        }

        public void Refresh()
        {
            _node.Refresh();
        }

        public void RegisterRefresh(Action<RedPointBaseNode> action)
        {
            _node.RegisterRefreshCallback(action);
        }

        public void UnRegisterRefresh(Action<RedPointBaseNode> action)
        {
            _node.UnRegisterRefreshCallback(action);
        }
    }
}