using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace SFramework.Utility.Runtime
{
    public static class DLog
    {
#if !UNITY_EDITOR
        [Conditional("ENABLE_DEBUG_LOG")]
#endif
        public static void Info(string log)
        {
            Debug.Log(log);
        }

#if !UNITY_EDITOR
        [Conditional("ENABLE_DEBUG_LOG")]
#endif
        [Conditional("ENABLE_DEBUG_LOG")]
        public static void Warning(string log)
        {
            Debug.LogWarning(log);
        }

#if !UNITY_EDITOR
        [Conditional("ENABLE_DEBUG_LOG")]
#endif
        [Conditional("ENABLE_DEBUG_LOG")]
        public static void Error(string log)
        {
            Debug.LogError(log);
        }
    }
}