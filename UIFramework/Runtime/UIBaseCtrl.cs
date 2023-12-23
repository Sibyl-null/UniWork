using UniWork.Utility.Runtime;
using UnityEngine;
using UniWork.UIFramework.Runtime.Scheduler;

namespace UniWork.UIFramework.Runtime
{
    public abstract class UIBaseCtrl
    {
        public UIBaseView UIView { get; private set; }
        public UIInfo Info { get; private set; }
        public bool IsShow { get; private set; }
        public bool EnableInput => UIManager.Instance.EnableInput && UIView.UIRaycaster.isActiveAndEnabled;

        public void Initialize(UIBaseView view, UIInfo info)
        {
            UIView = view;
            Info = info;

            SetUIScale();
            SetUIRenderLayer();
            
            OnCreate();
        }

        private void SetUIScale()
        {
            RectTransform rectTrans = UIView.GetComponent<RectTransform>();
            rectTrans.Overspread();
        }

        private void SetUIRenderLayer()
        {
            UIView.UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
            UIView.UICanvas.worldCamera = UIManager.Instance.UICamera;
            UIView.UICanvas.overrideSorting = true;
            UIView.UICanvas.sortingOrder = UIManager.Instance.GetLayerOrderWithIncrement(Info.LayerName);
        }

        /**
         * 一般用于初始化控件
         */
        protected virtual void OnCreate()
        {
        }

        public virtual void OnShow(UIBaseParameter param = null)
        {
            UIView.gameObject.SetActiveByClip(true);
            UIView.UICanvas.sortingOrder = UIManager.Instance.GetLayerOrderWithIncrement(Info.LayerName);
            IsShow = true;
        }

        public virtual void OnHide()
        {
            UIView.gameObject.SetActiveByClip(false);
            IsShow = false;
        }

        /**
         * 销毁对象，释放资源
         */
        public virtual void OnDestroy()
        {
        }

        public virtual void OnEscape()
        {
            UIManager.Instance.HideUI(Info.UIBaseType);
        }
    }
}