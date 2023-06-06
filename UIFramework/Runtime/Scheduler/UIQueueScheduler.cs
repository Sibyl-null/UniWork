using System.Collections.Generic;

namespace SFramework.UIFramework.Runtime.Scheduler
{
    internal sealed class UIQueueScheduler : UIBaseScheduler
    {
        private readonly Queue<UIEnumBaseType> _uiQueue = new Queue<UIEnumBaseType>();
        internal bool IsEmpty => _uiQueue.Count == 0;
        
        internal override void ShowUI(UIEnumBaseType uiEnumType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(uiEnumType);
            if (ctrl != null && ctrl.IsShow)
                return;
            
            if (_uiQueue.Count == 0)
                UIManager.Instance.ShowUIInternal(uiEnumType, param);
            _uiQueue.Enqueue(uiEnumType);
        }

        internal override void HideUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.HideUIInternal(uiEnumType);
            TryShowNextQueueUI(uiEnumType);
        }

        internal override void DestroyUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.DestroyUIInternal(uiEnumType);
            TryShowNextQueueUI(uiEnumType);
        }
        
        private void TryShowNextQueueUI(UIEnumBaseType uiEnumType)
        {
            if (_uiQueue.Count > 0 && _uiQueue.Peek() == uiEnumType)
            {
                _uiQueue.Dequeue();
                if (_uiQueue.Count > 0)
                    UIManager.Instance.ShowUIInternal(_uiQueue.Peek());
            }
        }
    }
}