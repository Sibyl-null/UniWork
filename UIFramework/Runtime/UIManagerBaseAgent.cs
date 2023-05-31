using System;
using System.Collections.ObjectModel;

namespace SFramework.UIFramework.Runtime
{
    public abstract class UIManagerBaseAgent
    {
        public abstract ReadOnlyCollection<UIEnumBaseLayer> GetAllLayers();
        public abstract void InitUIInfo(Action<UIEnumBaseType, UIInfo> addInfo);
        public abstract T Load<T>(string path) where T : UnityEngine.Object;
    }
}