using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UniWork.UIFramework.Runtime
{
    [RequireComponent(typeof(UIComponentCollector), typeof(Canvas), typeof(GraphicRaycaster))]
    public abstract class UIBaseView : MonoBehaviour
    {
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;
        private RectTransform _content;
        
        public Canvas UICanvas => _canvas;
        public GraphicRaycaster UIRaycaster => _raycaster;
        public RectTransform UIContent => _content;

        public void InitComponentRefs()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
            _content = transform.Find("Content").GetComponent<RectTransform>();

            var collector = GetComponent<UIComponentCollector>();
            InitCustomComponentRefs(collector.Components);
        }

        protected abstract void InitCustomComponentRefs(List<Object> components);
    }
}