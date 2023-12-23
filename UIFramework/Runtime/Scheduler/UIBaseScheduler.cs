using System;
using Cysharp.Threading.Tasks;

namespace UniWork.UIFramework.Runtime.Scheduler
{
    public abstract class UIBaseParameter{}
    
    internal abstract class UIBaseScheduler
    {
        internal abstract void ShowUI(Type ctrlType, UIBaseParameter param = null);
        internal abstract UniTask ShowUIAsync(Type ctrlType, UIBaseParameter param = null);
        internal abstract void HideUI(Type ctrlType);
        internal abstract void DestroyUI(Type ctrlType);
    }
}