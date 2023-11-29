using System;
using System.Collections.Generic;
using UnityEngine;
using UniWork.Utility.Runtime;

namespace UniWork.ObjectPool.Runtime
{
    public static class ObjectPool
    {
        internal static Func<string, GameObject> LoadAssetFunc;
        internal static Action<string> ReleaseAssetAction;
        internal static int DefaultInitCount;

        private static readonly Dictionary<string, ObjectItemFactory> _factories =
            new Dictionary<string, ObjectItemFactory>();

        private static Transform _objectPoolTrans;
        private static bool _hasInit = false;

        public static void Init(Func<string, GameObject> loadAssetFunc, Action<string> releaseAssetAction,
            int defaultInitCount = 20)
        {
            LoadAssetFunc = loadAssetFunc;
            DefaultInitCount = defaultInitCount;
            
            GameObject poolObj = new GameObject("ObjectPool");
            GameObject.DontDestroyOnLoad(poolObj);
            _objectPoolTrans = poolObj.transform;
            
            _hasInit = true;
        }

        private static void MakeSureInit()
        {
            if (_hasInit == false)
                throw new Exception("ObjectPool 未初始化");
            if (LoadAssetFunc == null)
                throw new Exception("ObjectPool LoadAssetFunc为null");
        }

        public static GameObject Get(string loadPath)
        {
            MakeSureInit();

            if (_factories.ContainsKey(loadPath) == false)
            {
                GameObject factoryObj = new GameObject();
                factoryObj.transform.SetParent(_objectPoolTrans);
                _factories.Add(loadPath, new ObjectItemFactory(loadPath, DefaultInitCount, factoryObj.transform));
            }

            return _factories[loadPath].Get();
        }

        public static void Recycle(string loadPath, GameObject obj)
        {
            MakeSureInit();

            if (_factories.TryGetValue(loadPath, out ObjectItemFactory factory))
                factory.Recycle(obj);
            else
                DLog.Error("不存在该对象池: " + loadPath);
        }

        public static void PreLoad(string loadPath, int initCount)
        {
            MakeSureInit();
            
            if (_factories.TryGetValue(loadPath, out ObjectItemFactory factory))
            {
                factory.PreLoad(initCount);
            }
            else
            {
                GameObject factoryObj = new GameObject();
                factoryObj.transform.SetParent(_objectPoolTrans);
                _factories.Add(loadPath, new ObjectItemFactory(loadPath, initCount, factoryObj.transform));
            }
        }

        public static void DestroyPool(string loadPath)
        {
            MakeSureInit();

            if (_factories.TryGetValue(loadPath, out ObjectItemFactory factory))
            {
                factory.Destroy();
                _factories.Remove(loadPath);
            }
            else
                DLog.Error("不存在该对象池: " + loadPath);
        }
    }
}