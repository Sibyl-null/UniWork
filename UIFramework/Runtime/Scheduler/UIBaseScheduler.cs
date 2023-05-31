namespace SFramework.UIFramework.Runtime.Scheduler
{
    internal abstract class UIBaseScheduler
    {
        internal abstract void ShowUI(UIEnumBaseType uiEnumType);
        internal abstract void HideUI(UIEnumBaseType uiEnumType);
        internal abstract void DestroyUI(UIEnumBaseType uiEnumType);
    }
}