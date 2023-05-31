namespace SFramework.UIFramework.Runtime.Scheduler
{
    internal sealed class UINormalScheduler : UIBaseScheduler
    {
        internal override void ShowUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.ShowUIInternal(uiEnumType);
        }

        internal override void HideUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.HideUIInternal(uiEnumType);
        }

        internal override void DestroyUI(UIEnumBaseType uiEnumType)
        {
            UIManager.Instance.DestroyUIInternal(uiEnumType);
        }
    }
}