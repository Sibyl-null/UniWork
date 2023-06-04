using UnityEngine;
using UnityEngine.UI;

namespace SFramework.UIFramework.Runtime
{
    [RequireComponent(typeof(Canvas), typeof(GraphicRaycaster), typeof(CanvasScaler))]
    public abstract class UIBaseView : MonoBehaviour
    {
        private Canvas _canvas;
        private GraphicRaycaster _raycaster;
        private CanvasScaler _scaler;
        
        public Canvas UICanvas => _canvas;
        public GraphicRaycaster UIRaycaster => _raycaster;
        public CanvasScaler UIScaler => _scaler;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _raycaster = GetComponent<GraphicRaycaster>();
            _scaler = GetComponent<CanvasScaler>();
        }
    }
}