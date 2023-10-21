using System;
using System.Collections;
using System.Collections.Generic;

namespace SFramework.Utility.Runtime.DataStructure
{
    public sealed class StackList<T> : IEnumerable<T>
    {
        private List<T> _list = new List<T>();

        public int Count => _list.Count;
        public int Capacity => _list.Capacity;

        public StackList()
        {
        }

        public StackList(int capacity)
        {
            _list.Capacity = capacity;
        }

        public void Push(T value)
        {
            _list.Add(value);
        }

        public T Pop()
        {
            T t = Peek();
            _list.RemoveAt(Count - 1);
            return t;
        }

        public T Peek()
        {
            if (Count == 0)
                throw new Exception("StackList 为空，禁止Pop/Peek");

            return _list[Count - 1];
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= Count)
                throw new Exception("StackList RemoveAt 下标越界");
            
            _list.RemoveAt(index);
        }

        public void Remove(T value)
        {
            _list.Remove(value);
        }

        public bool Contains(T value)
        {
            return _list.Contains(value);
        }


        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _list.Count - 1; i >= 0; --i)
                yield return _list[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
