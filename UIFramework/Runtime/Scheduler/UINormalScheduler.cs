using Cysharp.Threading.Tasks;

namespace UniWork.UIFramework.Runtime.Scheduler
{
    internal sealed class UINormalScheduler : UIBaseScheduler
    {
        internal override void ShowUI(UIBaseType uiType, UIBaseParameter param = null)
        {
            UIManager.Instance.ShowUIInternal(uiType, param);
        }

        internal override async UniTask ShowUIAsync(UIBaseType uiType, UIBaseParameter param = null)
        {
            await UIManager.Instance.ShowUIAsyncInternal(uiType, param);
        }

        internal override void HideUI(UIBaseType uiType)
        {
            UIManager.Instance.HideUIInternal(uiType);
        }

        internal override void DestroyUI(UIBaseType uiType)
        {
            UIManager.Instance.DestroyUIInternal(uiType);
        }
    }
}