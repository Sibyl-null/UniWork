using System.Collections.Generic;

namespace SFramework.UIFramework.Runtime.Scheduler
{
    internal sealed class UIStackScheduler : UIBaseScheduler
    {
        private readonly Stack<UIEnumBaseType> _uiStack = new Stack<UIEnumBaseType>();
        internal bool IsEmpty => _uiStack.Count == 0;
        
        internal override void ShowUI(UIEnumBaseType uiEnumType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(uiEnumType);
            if (ctrl != null && ctrl.IsShow)
                return;

            if (_uiStack.Count > 0)
                UIManager.Instance.HideUIInternal(_uiStack.Peek());
            
            _uiStack.Push(uiEnumType);
            UIManager.Instance.ShowUIInternal(uiEnumType, param);
        }

        internal override void HideUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.HideUIInternal(uiEnumType);
            TryShowNextStackUI(uiEnumType);
        }

        internal override void DestroyUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.DestroyUIInternal(uiEnumType);
            TryShowNextStackUI(uiEnumType);
        }

        internal void EscapeUI()
        {
            UIManager.Instance.GetUICtrl(_uiStack.Peek()).OnEscape();
        }
        
        private void TryShowNextStackUI(UIEnumBaseType uiEnumType)
        {
            if (_uiStack.Count > 0 && _uiStack.Peek() == uiEnumType)
            {
                _uiStack.Pop();
                if (_uiStack.Count > 0)
                    UIManager.Instance.ShowUIInternal(_uiStack.Peek());
            }
        }
    }
}