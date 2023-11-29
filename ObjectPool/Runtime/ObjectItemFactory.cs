using System.Collections.Generic;
using UnityEngine;

namespace UniWork.ObjectPool.Runtime
{
    internal class ObjectItemFactory
    {
        private Transform _factoryTrans;
        private readonly Queue<GameObject> _queue;
        private GameObject _objectPrefab;
        private string _loadPath;

        public ObjectItemFactory(string loadPath, int initCount, Transform factoryTrans)
        {
            _factoryTrans = factoryTrans;
            _factoryTrans.name = "[Factory] " + loadPath;
            _loadPath = loadPath;
            _queue = new Queue<GameObject>(initCount);
            _objectPrefab = ObjectPool.LoadAssetFunc.Invoke(loadPath);
            PreLoad(initCount);
        }

        public void PreLoad(int count)
        {
            if (count <= _queue.Count)
                return;

            int needCount = count - _queue.Count;
            for (int i = 0; i < needCount; ++i)
            {
                GameObject obj = GameObject.Instantiate(_objectPrefab, _factoryTrans);
                obj.SetActive(false);
                _queue.Enqueue(obj);
            }
        }

        public GameObject Get()
        {
            if (_queue.Count > 0)
            {
                GameObject obj = _queue.Dequeue();
                obj.SetActive(true);
                return obj;
            }

            return GameObject.Instantiate(_objectPrefab, _factoryTrans);
        }

        public void Recycle(GameObject obj)
        {
            obj.SetActive(false);
            obj.transform.SetParent(_factoryTrans);
            _queue.Enqueue(obj);
        }

        public void Destroy()
        {
            _queue.Clear();
            GameObject.Destroy(_factoryTrans.gameObject);
            ObjectPool.ReleaseAssetAction?.Invoke(_loadPath);
            _objectPrefab = null;
        }
    }
}