using System;
using UnityEngine;
using UnityEngine.UI;

namespace SFramework.UIFramework.Runtime
{
    public abstract class UIBaseView : MonoBehaviour
    {
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;
        
        public Canvas UICanvas => _canvas;
        public GraphicRaycaster UIRaycaster => _raycaster;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
            if (_canvas == null || _raycaster == null)
                throw new Exception("UIView 没有 Canvas 或 GraphicRaycaster 组件");
        }
    }
}