using System;
using System.Collections.Generic;

namespace SFramework.ReferencePool.Runtime
{
    internal class ReferenceCollector
    {
        private readonly Stack<IReferenceRelease> _releases;

        public Type ReferenceType { get; private set; }
        public int OutSideCount { get; private set; } = 0;
        public int InSideCount { get; private set; } = 0;

        public ReferenceCollector(Type referenceType, int capacity)
        {
            if (typeof(IReferenceRelease).IsAssignableFrom(referenceType) == false)
                throw new Exception(referenceType.Name + "不是IReferenceRelease的子类");
            
            ReferenceType = referenceType;
            _releases = new Stack<IReferenceRelease>(capacity);
        }

        public IReferenceRelease Spawn()
        {
            ++OutSideCount;

            if (InSideCount > 0)
            {
                --InSideCount;
                return _releases.Pop();
            }

            return Activator.CreateInstance(ReferenceType) as IReferenceRelease;
        }

        public void Release(IReferenceRelease release)
        {
            if (release.GetType() != ReferenceType)
                throw new Exception($"引用池回收出错：{release.GetType().Name} 和 {ReferenceType.Name} 类型不匹配");

            --OutSideCount;
            ++InSideCount;
            release.Release();
            _releases.Push(release);
        }
    }
}