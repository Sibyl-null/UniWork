using System;
using Cysharp.Threading.Tasks;
using UniWork.Utility.Runtime;
using UniWork.Utility.Runtime.DataStructure;

namespace UniWork.UIFramework.Runtime.Scheduler
{
    internal sealed class UIStackScheduler : UIBaseScheduler
    {
        private readonly StackList<Type> _ctrlTypeStack = new();
        
        internal StackList<Type> CtrlTypeStack => _ctrlTypeStack;
        internal bool IsEmpty => _ctrlTypeStack.Count == 0;

        internal override void ShowUI(Type ctrlType, UIBaseParameter param = null)
        {
            // 约定（保证）栈顶的元素一定是显示的
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(ctrlType);
            if (ctrl != null && ctrl.IsShow)
                return;

            if (_ctrlTypeStack.Count > 0)
                UIManager.Instance.HideUIInternal(_ctrlTypeStack.Peek());
            
            // 若要打开的是非栈顶元素，先从栈中移除，防止重复进栈。
            if (_ctrlTypeStack.Contains(ctrlType))
                _ctrlTypeStack.Remove(ctrlType);
                
            _ctrlTypeStack.Push(ctrlType);
            UIManager.Instance.ShowUIInternal(ctrlType, param);
        }

        internal override async UniTask ShowUIAsync(Type ctrlType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(ctrlType);
            if (ctrl != null && ctrl.IsShow)
                return;

            if (_ctrlTypeStack.Count > 0)
                UIManager.Instance.HideUIInternal(_ctrlTypeStack.Peek());
            
            if (_ctrlTypeStack.Contains(ctrlType))
                _ctrlTypeStack.Remove(ctrlType);
                
            _ctrlTypeStack.Push(ctrlType);
            await UIManager.Instance.ShowUIAsyncInternal(ctrlType, param);
        }

        internal override void HideUI(Type ctrlType)
        {
            UIManager.Instance.HideUIInternal(ctrlType);
            TryShowNextStackUI(ctrlType);
        }

        internal override void DestroyUI(Type ctrlType)
        {
            UIManager.Instance.DestroyUIInternal(ctrlType);
            
            if (_ctrlTypeStack.Count > 0 && _ctrlTypeStack.Peek() != ctrlType)
                _ctrlTypeStack.Remove(ctrlType);
            else
                TryShowNextStackUI(ctrlType);
        }

        internal void EscapeUI()
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(_ctrlTypeStack.Peek());
            if (ctrl.EnableInput == false)
            {
                DLog.Info("[UIManager] Ctrl Input 禁用中，返回键无效. CtrlType: " + ctrl.GetType().Name);
                return;
            }
            
            ctrl.OnEscape();
        }
        
        private void TryShowNextStackUI(Type ctrlType)
        {
            if (_ctrlTypeStack.Count > 0 && _ctrlTypeStack.Peek() == ctrlType)
            {
                _ctrlTypeStack.Pop();
                if (_ctrlTypeStack.Count > 0)
                    UIManager.Instance.ShowUIInternal(_ctrlTypeStack.Peek());
            }
        }
    }
}