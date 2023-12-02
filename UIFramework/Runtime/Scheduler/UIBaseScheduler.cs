using Cysharp.Threading.Tasks;

namespace UniWork.UIFramework.Runtime.Scheduler
{
    public abstract class UIBaseParameter{}
    
    internal abstract class UIBaseScheduler
    {
        internal abstract void ShowUI(UIBaseType uiType, UIBaseParameter param = null);
        internal abstract UniTask ShowUIAsync(UIBaseType uiType, UIBaseParameter param = null);
        internal abstract void HideUI(UIBaseType uiType);
        internal abstract void DestroyUI(UIBaseType uiType);
    }
}