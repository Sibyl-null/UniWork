using UniWork.Utility.Runtime;
using UnityEngine;

namespace UniWork.UIFramework.Runtime
{
    public abstract class UIBaseParameter{}
    
    public abstract class UIBaseCtrl
    {
        public UIBaseView UIView { get; private set; }
        public UIInfo Info { get; private set; }
        public bool IsShow { get; private set; }
        public bool EnableInput => UIManager.Instance.EnableInput && UIView.UIRaycaster.isActiveAndEnabled;

        public void Create(UIBaseView view, UIInfo info)
        {
            UIView = view;
            Info = info;

            SetUIScale();
            SetUIRenderLayer();
            
            OnCreate();
        }

        public void Show(UIBaseParameter param = null)
        {
            UIView.gameObject.SetActiveByClip(true);
            UIView.UICanvas.sortingOrder = UIManager.Instance.GetLayerOrderWithIncrement(Info.LayerName);
            IsShow = true;

            OnShow(param);
        }

        public void Hide()
        {
            UIView.gameObject.SetActiveByClip(false);
            IsShow = false;

            OnHide();
        }

        public void Destroy()
        {
            OnDestroy();
            Object.Destroy(UIView.gameObject);
        }

        private void SetUIScale()
        {
            RectTransform rectTrans = UIView.GetComponent<RectTransform>();
            rectTrans.Overspread();
        }

        private void SetUIRenderLayer()
        {
            UIView.UICanvas.overrideSorting = true;
            UIView.UICanvas.sortingLayerID = UIManager.Instance.SortingLayerId;
            UIView.UICanvas.sortingOrder = UIManager.Instance.GetLayerOrderWithIncrement(Info.LayerName);
        }
        
        
        // ----------------------------------------------------------------------------
        // virtual methods
        // ----------------------------------------------------------------------------
        
        protected virtual void OnCreate()
        {
        }

        protected virtual void OnShow(UIBaseParameter param = null)
        {
        }

        protected virtual void OnHide()
        {
        }
        
        protected virtual void OnDestroy()
        {
        }

        /// <summary>
        /// 按下返回键的行为，只有调度模式为 Stack 的才会生效
        /// </summary>
        public virtual void OnEscape()
        {
            UIManager.Instance.HideUI(this.GetType());
        }
    }
}