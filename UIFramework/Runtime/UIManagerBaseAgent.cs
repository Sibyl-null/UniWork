using System.Collections.ObjectModel;

namespace UniWork.UIFramework.Runtime
{
    public abstract class UIManagerBaseAgent
    {
        public abstract string UIRootLoadPath { get; }
        public virtual int LayerOrderOnceRaise => 10;
        
        protected void AddInfo(UIBaseType uiType, UIInfo info)
        {
            UIManager.Instance.AddInfo(uiType, info);
        }
        
        public abstract ReadOnlyCollection<UIBaseLayer> GetAllLayers();
        public abstract void InitUIInfo();
        public abstract T Load<T>(string path) where T : UnityEngine.Object;
        public abstract void UnLoad(string path);
    }
}