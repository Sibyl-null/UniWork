using System;
using Cysharp.Threading.Tasks;

namespace UniWork.UIFramework.Runtime.Scheduler.Implements
{
    internal sealed class UINormalScheduler : UIBaseScheduler
    {
        internal override void ShowUI(Type ctrlTyp, UIBaseParameter param = null)
        {
            UIManager.Instance.ShowUIInternal(ctrlTyp, param);
        }

        internal override async UniTask ShowUIAsync(Type ctrlTyp, UIBaseParameter param = null)
        {
            await UIManager.Instance.ShowUIAsyncInternal(ctrlTyp, param);
        }

        internal override void HideUI(Type ctrlTyp)
        {
            UIManager.Instance.HideUIInternal(ctrlTyp);
        }

        internal override void DestroyUI(Type ctrlTyp)
        {
            UIManager.Instance.DestroyUIInternal(ctrlTyp);
        }
    }
}