using Cysharp.Threading.Tasks;

namespace UniWork.UIFramework.Runtime
{
    public abstract class UIBaseAgent
    {
        public abstract string RuntimeSettingLoadPath { get; }
        
        protected void AddInfo(UIInfo info)
        {
            UIManager.Instance.AddInfo(info);
        }
        
        public abstract void InitUIInfo();
        
        public abstract T Load<T>(string path) where T : UnityEngine.Object;
        public abstract UniTask<T> LoadAsync<T>(string path) where T : UnityEngine.Object;
        public abstract void UnLoad(string path);
    }
}