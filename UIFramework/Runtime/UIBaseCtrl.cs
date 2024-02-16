using Cysharp.Threading.Tasks;
using UniWork.Utility.Runtime;
using UnityEngine;

namespace UniWork.UIFramework.Runtime
{
    public abstract class UIBaseParameter{}
    
    public abstract class UIBaseCtrl
    {
        public UIBaseView View { get; private set; }
        public UIInfo Info { get; private set; }
        public bool IsShow { get; private set; }

        public bool EnableInput
        {
            get => UIManager.Instance.EnableInput && View.UIRaycaster.isActiveAndEnabled;
            private set => View.UIRaycaster.enabled = value;
        }

        public void Create(UIBaseView view, UIInfo info)
        {
            View = view;
            Info = info;

            View.InitComponentRefs();
            SetUIScale();
            SetUIRenderLayer();
            
            OnCreate();
        }

        public async UniTaskVoid Show(UIBaseParameter param = null)
        {
            if (IsShow)
                return;
            
            View.gameObject.SetActiveByClip(true);
            View.UICanvas.sortingOrder = UIManager.Instance.GetLayerOrderWithIncrement(Info.LayerName);
            IsShow = true;
            OnShow(param);

            EnableInput = false;
            await ShowAnimPlay();
            EnableInput = true;
        }

        public async UniTask Hide()
        {
            if (IsShow == false)
                return;

            EnableInput = false;
            await HideAnimPlay();
            EnableInput = true;

            View.gameObject.SetActiveByClip(false);
            IsShow = false;
            OnHide();
        }

        public async UniTaskVoid Destroy()
        {
            await Hide();
            OnDestroy();
            Object.Destroy(View.gameObject);
        }

        private void SetUIScale()
        {
            RectTransform rectTrans = View.GetComponent<RectTransform>();
            rectTrans.Overspread();
        }

        private void SetUIRenderLayer()
        {
            View.UICanvas.overrideSorting = true;
            View.UICanvas.sortingLayerID = UIManager.Instance.SortingLayerId;
            View.UICanvas.sortingOrder = UIManager.Instance.GetLayerOrderWithIncrement(Info.LayerName);
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

        protected virtual UniTask ShowAnimPlay()
        {
            return UniTask.CompletedTask;
        }

        protected virtual UniTask HideAnimPlay()
        {
            return UniTask.CompletedTask;
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