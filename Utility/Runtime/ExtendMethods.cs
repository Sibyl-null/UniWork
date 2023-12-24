using System;
using UnityEngine;

namespace UniWork.Utility.Runtime
{
    public static class ExtendMethods
    {
        public static void DLogArrayInfos(this Array arr)
        {
            foreach (object o in arr)
                DLog.Info(o.ToString());
        }
        
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            return (T)obj.GetOrAddComponent(typeof(T));
        }
        
        public static Component GetOrAddComponent(this GameObject obj, Type type)
        {
            if (typeof(Component).IsAssignableFrom(type) == false)
                throw new Exception($"[Utility] {type.Name} 不是 Component 的子类");
            
            Component t = obj.GetComponent(type);
            
            if (t == null)
                t = obj.AddComponent(type);
            
            return t;
        }

        /// <summary>
        /// UI 控件布局设置为全屏
        /// </summary>
        public static void Overspread(this RectTransform rectTrans)
        {
            rectTrans.pivot = 0.5f * Vector2.one;
            rectTrans.localScale = Vector3.one;
            rectTrans.offsetMin = Vector2.zero;
            rectTrans.offsetMax = Vector2.zero;
            rectTrans.anchorMin = Vector2.zero;
            rectTrans.anchorMax = Vector2.one;
        }

        public static void SetActiveByClip(this GameObject go, bool active)
        {
            if (active && go.activeSelf == false)
                go.SetActive(true);

            Transform trans = go.transform;
            Vector3 localPos = trans.localPosition;
            localPos.z = active ? 0f : -1000f;
            trans.localPosition = localPos;
        }
    }
}