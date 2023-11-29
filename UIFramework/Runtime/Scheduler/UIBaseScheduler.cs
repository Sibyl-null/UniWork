namespace UniWork.UIFramework.Runtime.Scheduler
{
    public abstract class UIBaseParameter{}
    
    internal abstract class UIBaseScheduler
    {
        internal abstract void ShowUI(UIEnumBaseType uiEnumType, UIBaseParameter param = null);
        internal abstract void HideUI(UIEnumBaseType uiEnumType);
        internal abstract void DestroyUI(UIEnumBaseType uiEnumType);
    }
}