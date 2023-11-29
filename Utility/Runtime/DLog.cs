using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace UniWork.Utility.Runtime
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
        public static void Warning(string log)
        {
            Debug.LogWarning(log);
        }

#if !UNITY_EDITOR
        [Conditional("ENABLE_DEBUG_LOG")]
#endif
        public static void Error(string log)
        {
            Debug.LogError(log);
        }
    }
}