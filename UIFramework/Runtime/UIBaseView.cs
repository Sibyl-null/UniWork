using UnityEngine;
using UnityEngine.UI;

namespace UniWork.UIFramework.Runtime
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster))]
    public abstract class UIBaseView : MonoBehaviour
    {
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;
        private RectTransform _content;

        public Canvas UICanvas => _canvas;
        public GraphicRaycaster UIRaycaster => _raycaster;
        public RectTransform UIContent => _content;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
            _content = transform.Find("Content").GetComponent<RectTransform>();
        }
    }
}