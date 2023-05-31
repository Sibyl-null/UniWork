using UnityEngine;

namespace SFramework.Utility
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
    }
}