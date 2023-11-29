using System;
using System.IO;
using MemoryPack;
using UnityEngine;
using UniWork.Utility.Runtime;

namespace UniWork.Store.Runtime
{
    public static class StoreRoot
    {
        public static readonly string StorePath = Path.Combine(Application.persistentDataPath, "Store");
        
        public static StoreBaseContainer BaseContainer { get; private set; }
        private static Type ContainerType { get; set; }
        
        public static void Create(Type containerType)
        {
            if (BaseContainer != null)
            {
                DLog.Error("[Store] 禁止重复调用 Create");
                return;
            }

            if (typeof(StoreBaseContainer).IsAssignableFrom(containerType) == false)
            {
                DLog.Error("[Store] containerType 不是 StoreBaseContainer 的子类");
                return;
            }

            ContainerType = containerType;
            
            if (File.Exists(StorePath))
            {
                byte[] bytes = File.ReadAllBytes(StorePath);
                BaseContainer = (StoreBaseContainer)MemoryPackSerializer.Deserialize(ContainerType, bytes);
                DLog.Info("[Store] StoreRoot.Create Success For Store");
            }
            else
            {
                BaseContainer = (StoreBaseContainer)Activator.CreateInstance(ContainerType);
                BaseContainer.Init();
                DLog.Info("[Store] StoreRoot.Create Success For New");
            }
        }

        public static void Release()
        {
            if (CheckIsReady() == false)
                return;
            
            BaseContainer.Reset();

            if (File.Exists(StorePath))
                File.Delete(StorePath);
        }

        public static void Save()
        {
            if (CheckIsReady() == false)
                return;
            
            byte[] bytes = MemoryPackSerializer.Serialize(ContainerType, BaseContainer);
            File.WriteAllBytes(StorePath, bytes);
            DLog.Info($"[Store] 存档保存成功");
        }

        private static bool CheckIsReady()
        {
            if (BaseContainer == null)
            {
                DLog.Error("[Store] Container is Null!");
                return false;
            }

            return true;
        }
    }
}