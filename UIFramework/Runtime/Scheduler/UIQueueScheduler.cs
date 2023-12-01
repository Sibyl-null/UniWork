using System.Collections.Generic;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Runtime.Scheduler
{
    internal sealed class UIQueueScheduler : UIBaseScheduler
    {
        private readonly Queue<UIBaseType> _uiQueue = new Queue<UIBaseType>();

        internal Queue<UIBaseType> UiQueue => _uiQueue;
        internal bool IsEmpty => _uiQueue.Count == 0;
        
        internal override void ShowUI(UIBaseType uiType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(uiType);
            if (ctrl != null && ctrl.IsShow)
                return;
            
            if (_uiQueue.Count == 0)
                UIManager.Instance.ShowUIInternal(uiType, param);
            _uiQueue.Enqueue(uiType);
        }

        internal override void HideUI(UIBaseType uiType)
        {
            if (_uiQueue.Count > 0 && _uiQueue.Peek() != uiType)
            {
                DLog.Error("UI 队列调用: 不允许隐藏非队头元素");
                return;
            }
            
            UIManager.Instance.HideUIInternal(uiType);
            TryShowNextQueueUI(uiType);
        }

        internal override void DestroyUI(UIBaseType uiType)
        {
            if (_uiQueue.Count > 0 && _uiQueue.Peek() != uiType)
            {
                DLog.Error("UI 队列调用: 不允许销毁非队头元素");
                return;
            }
            
            UIManager.Instance.DestroyUIInternal(uiType);
            TryShowNextQueueUI(uiType);
        }
        
        private void TryShowNextQueueUI(UIBaseType uiType)
        {
            if (_uiQueue.Count > 0 && _uiQueue.Peek() == uiType)
            {
                _uiQueue.Dequeue();
                if (_uiQueue.Count > 0)
                    UIManager.Instance.ShowUIInternal(_uiQueue.Peek());
            }
        }
    }
}