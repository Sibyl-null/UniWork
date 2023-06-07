using System;
using System.Collections.Generic;

namespace SFramework.ReferencePool.Runtime
{
    public static class ReferencePool
    {
        public static int InitCapacity = 30;
        private static readonly Dictionary<Type, ReferenceCollector> _collectors = new();

        public static T Spawn<T>() where T : class, IReferenceRelease, new()
        {
            Type type = typeof(T);
            if (!_collectors.ContainsKey(type))
                _collectors.Add(type, new ReferenceCollector(type, InitCapacity));

            return _collectors[type].Spawn() as T;
        }

        public static void Release(IReferenceRelease release)
        {
            Type referenceType = release.GetType();
            if (!_collectors.ContainsKey(referenceType))
                throw new Exception($"不存在{referenceType.Name}类型的缓存池");

            _collectors[referenceType].Release(release);
        }

        public static void ClearAll()
        {
            _collectors.Clear();
        }
    }
}