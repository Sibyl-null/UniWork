using System;
using UnityEngine;

namespace Utility.Runtime
{
    public static class ExtendMethods
    {
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T t = obj.GetComponent<T>();
            
            if (t == null)
                t = obj.AddComponent<T>();
            
            return t;
        }

        public static void DLogArrayInfos(this Array arr)
        {
            foreach (object o in arr)
                DLog.Info(o.ToString());
        }
    }
}