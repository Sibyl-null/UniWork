using System;
using System.Collections.Generic;
using UnityEditor.Build;

namespace UniWork.UniBundle.Editor.BundleBuilder.BuildContexts
{
    internal interface IContextData
    {
    }

    internal static class BuildContext
    {
        private static readonly Dictionary<Type, IContextData> _contextDataDic =
            new Dictionary<Type, IContextData>();

        internal static void SetContextData(IContextData contextData)
        {
            if (contextData == null)
                throw new BuildFailedException("ContextData is null");

            Type type = contextData.GetType();
            if (_contextDataDic.ContainsKey(type))
                throw new BuildFailedException($"ContextData: {type.Name} 已在上下文存在");
            
            _contextDataDic.Add(type, contextData);
        }

        internal static T GetContextData<T>() where T : class, IContextData
        {
            Type type = typeof(T);
            if (_contextDataDic.ContainsKey(type) == false)
                throw new BuildFailedException($"ContextData: {type.Name} 在上下文中不存在");

            T data = _contextDataDic[type] as T;
            if (data == null)
                throw new BuildFailedException($"ContextData: {type.Name} 为空");

            return data;
        }
        
        internal static void Clear()
        {
            _contextDataDic.Clear();
        }
    }
}