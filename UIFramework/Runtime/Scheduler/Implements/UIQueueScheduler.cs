using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniWork.Utility.Runtime;

namespace UniWork.UIFramework.Runtime.Scheduler.Implements
{
    internal sealed class UIQueueScheduler : UIBaseScheduler
    {
        private readonly Queue<Type> _ctrlTypeQueue = new();

        internal Queue<Type> CtrlTypeQueue => _ctrlTypeQueue;
        internal bool IsEmpty => _ctrlTypeQueue.Count == 0;
        
        internal override void ShowUI(Type ctrlType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(ctrlType);
            if (ctrl != null && ctrl.IsShow)
                return;
            
            if (_ctrlTypeQueue.Count == 0)
                UIManager.Instance.ShowUIInternal(ctrlType, param);
            _ctrlTypeQueue.Enqueue(ctrlType);
        }

        internal override async UniTask ShowUIAsync(Type ctrlType, UIBaseParameter param = null)
        {
            UIBaseCtrl ctrl = UIManager.Instance.GetUICtrl(ctrlType);
            if (ctrl != null && ctrl.IsShow)
                return;

            if (_ctrlTypeQueue.Count == 0)
                await UIManager.Instance.ShowUIAsyncInternal(ctrlType, param);
            
            _ctrlTypeQueue.Enqueue(ctrlType);
        }

        internal override void HideUI(Type ctrlType)
        {
            if (_ctrlTypeQueue.Count > 0 && _ctrlTypeQueue.Peek() != ctrlType)
            {
                DLog.Error("UI 队列调用: 不允许隐藏非队头元素");
                return;
            }
            
            UIManager.Instance.HideUIInternal(ctrlType);
            TryShowNextQueueUI(ctrlType);
        }

        internal override void DestroyUI(Type ctrlType)
        {
            if (_ctrlTypeQueue.Count > 0 && _ctrlTypeQueue.Peek() != ctrlType)
            {
                DLog.Error("UI 队列调用: 不允许销毁非队头元素");
                return;
            }
            
            UIManager.Instance.DestroyUIInternal(ctrlType);
            TryShowNextQueueUI(ctrlType);
        }
        
        private void TryShowNextQueueUI(Type ctrlType)
        {
            if (_ctrlTypeQueue.Count > 0 && _ctrlTypeQueue.Peek() == ctrlType)
            {
                _ctrlTypeQueue.Dequeue();
                if (_ctrlTypeQueue.Count > 0)
                    UIManager.Instance.ShowUIInternal(_ctrlTypeQueue.Peek());
            }
        }
    }
}