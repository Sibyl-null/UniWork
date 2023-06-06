using UnityEngine;
using UnityEngine.UI;

namespace SFramework.UIFramework.Runtime
{
    public abstract class UIBaseCtrl
    {
        protected UIBaseView _uiView;
        public UIInfo Info { get; private set; }
        public bool IsShow => _uiView.UICanvas.enabled && _uiView.gameObject.activeSelf;

        public void Initialize(UIBaseView view, UIInfo info)
        {
            _uiView = view;
            Info = info;
            
            SetUIRenderLayer();
            SetUIScaler();
            
            OnCreate();
        }

        private void SetUIRenderLayer()
        {
            _uiView.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
            _uiView.UICanvas.worldCamera = UIManager.Instance.UICamera;
            _uiView.UICanvas.sortingOrder = Info.UIEnumBaseLayer.key + UIManager.Instance.OrderLayerIncrement;
        }

        private void SetUIScaler()
        {
            UIRuntimeSetting runtimeSetting = UIManager.Instance.RuntimeSetting;
            
            _uiView.UIScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _uiView.UIScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _uiView.UIScaler.referenceResolution = new Vector2(runtimeSetting.width, runtimeSetting.height);
            _uiView.UIScaler.matchWidthOrHeight = runtimeSetting.match;
        }

        /*
         * 一般用于初始化控件
         */
        protected virtual void OnCreate()
        {
        }

        public virtual void OnShow()
        {
            _uiView.UICanvas.enabled = true;
            _uiView.UIRaycaster.enabled = true;
            _uiView.UICanvas.sortingOrder = Info.UIEnumBaseLayer.key + UIManager.Instance.OrderLayerIncrement;
        }

        public virtual void OnHide()
        {
            _uiView.UICanvas.enabled = false;
            _uiView.UIRaycaster.enabled = false;
        }

        /*
         * 销毁对象，释放资源
         */
        public virtual void OnDestroy()
        {
            GameObject.Destroy(_uiView.gameObject);
        }

        public virtual void OnEscape()
        {
            UIManager.Instance.HideUI(Info.UIEnumBaseType);
        }
    }
}