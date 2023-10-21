using Utility.Runtime.DataStructure;

namespace UIFramework.Runtime.Scheduler
{
    internal sealed class UIStackScheduler : UIBaseScheduler
    {
        private readonly StackList<UIEnumBaseType> _uiStack = new StackList<UIEnumBaseType>();
        
        internal StackList<UIEnumBaseType> UiStack => _uiStack;
        internal bool IsEmpty => _uiStack.Count == 0;

        internal override void ShowUI(UIEnumBaseType uiEnumType, UIBaseParameter param = null)
        {
            // 约定（保证）栈顶的元素一定是显示的
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(uiEnumType);
            if (ctrl != null && ctrl.IsShow)
                return;

            if (_uiStack.Count > 0)
                UIManager.Instance.HideUIInternal(_uiStack.Peek());
            
            // 若要打开的是非栈顶元素，先从栈中移除，防止重复进栈。
            if (_uiStack.Contains(uiEnumType))
                _uiStack.Remove(uiEnumType);
                
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
            
            if (_uiStack.Count > 0 && _uiStack.Peek() != uiEnumType)
                _uiStack.Remove(uiEnumType);
            else
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