using UniWork.Utility.Runtime.DataStructure;

namespace UniWork.UIFramework.Runtime.Scheduler
{
    internal sealed class UIStackScheduler : UIBaseScheduler
    {
        private readonly StackList<UIBaseType> _uiStack = new StackList<UIBaseType>();
        
        internal StackList<UIBaseType> UiStack => _uiStack;
        internal bool IsEmpty => _uiStack.Count == 0;

        internal override void ShowUI(UIBaseType uiType, UIBaseParameter param = null)
        {
            // 约定（保证）栈顶的元素一定是显示的
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(uiType);
            if (ctrl != null && ctrl.IsShow)
                return;

            if (_uiStack.Count > 0)
                UIManager.Instance.HideUIInternal(_uiStack.Peek());
            
            // 若要打开的是非栈顶元素，先从栈中移除，防止重复进栈。
            if (_uiStack.Contains(uiType))
                _uiStack.Remove(uiType);
                
            _uiStack.Push(uiType);
            UIManager.Instance.ShowUIInternal(uiType, param);
        }

        internal override void HideUI(UIBaseType uiType)
        {
            UIManager.Instance.HideUIInternal(uiType);
            TryShowNextStackUI(uiType);
        }

        internal override void DestroyUI(UIBaseType uiType)
        {
            UIManager.Instance.DestroyUIInternal(uiType);
            
            if (_uiStack.Count > 0 && _uiStack.Peek() != uiType)
                _uiStack.Remove(uiType);
            else
                TryShowNextStackUI(uiType);
        }

        internal void EscapeUI()
        {
            UIManager.Instance.GetUICtrl(_uiStack.Peek()).OnEscape();
        }
        
        private void TryShowNextStackUI(UIBaseType uiType)
        {
            if (_uiStack.Count > 0 && _uiStack.Peek() == uiType)
            {
                _uiStack.Pop();
                if (_uiStack.Count > 0)
                    UIManager.Instance.ShowUIInternal(_uiStack.Peek());
            }
        }
    }
}