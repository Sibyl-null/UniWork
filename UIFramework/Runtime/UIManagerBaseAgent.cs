using System.Collections.ObjectModel;

namespace SFramework.UIFramework.Runtime
{
    public abstract class UIManagerBaseAgent
    {
        public abstract string UIRootLoadPath { get; }
        public virtual int LayerOrderOnceRaise => 10;
        
        protected void AddInfo(UIEnumBaseType uiEnumType, UIInfo info)
        {
            UIManager.Instance.AddInfo(uiEnumType, info);
        }
        
        public abstract ReadOnlyCollection<UIEnumBaseLayer> GetAllLayers();
        public abstract void InitUIInfo();
        public abstract T Load<T>(string path) where T : UnityEngine.Object;
    }
}