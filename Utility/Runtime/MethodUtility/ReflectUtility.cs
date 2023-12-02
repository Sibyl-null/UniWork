using System;
using System.Reflection;

namespace UniWork.Utility.Runtime.MethodUtility
{
    public static class ReflectUtility
    {
        public static object InvokeNonPublicStaticMethod(Type type, string method, params object[] parameters)
        {
            MethodInfo methodInfo = type.GetMethod(method, BindingFlags.NonPublic | BindingFlags.Static);
            if (methodInfo == null)
            {
                DLog.Error($"{type.FullName} not found method : {method}");
                return null;
            }

            return methodInfo.Invoke(null, parameters);
        }
    }
}