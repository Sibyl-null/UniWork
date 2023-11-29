using System;
using System.Collections.Generic;

namespace UniWork.Message.Runtime
{
    public class Message : MessageBase
    {
        public void Register(Action ac)
        {
            RegisterInternal(ac);
        }

        public void UnRegister(Action ac)
        {
            UnRegisterInternal(ac);
        }

        public void BroadCast()
        {
            if (_delegates.Count == 0)
                return;

            LinkedListNode<Delegate> curNode = _delegates.First;
            while (curNode != null)
            {
                ((Action)curNode.Value).Invoke();
                curNode = curNode.Next;
            }
        }
    }
    
    public class Message<T1> : MessageBase
    {
        public void Register(Action<T1> ac)
        {
            RegisterInternal(ac);
        }

        public void UnRegister(Action<T1> ac)
        {
            UnRegisterInternal(ac);
        }

        public void BroadCast(T1 t1)
        {
            if (_delegates.Count == 0)
                return;

            LinkedListNode<Delegate> curNode = _delegates.First;
            while (curNode != null)
            {
                ((Action<T1>)curNode.Value).Invoke(t1);
                curNode = curNode.Next;
            }
        }
    }
    
    public class Message<T1, T2> : MessageBase
    {
        public void Register(Action<T1, T2> ac)
        {
            RegisterInternal(ac);
        }

        public void UnRegister(Action<T1, T2> ac)
        {
            UnRegisterInternal(ac);
        }

        public void BroadCast(T1 t1, T2 t2)
        {
            if (_delegates.Count == 0)
                return;

            LinkedListNode<Delegate> curNode = _delegates.First;
            while (curNode != null)
            {
                ((Action<T1, T2>)curNode.Value).Invoke(t1, t2);
                curNode = curNode.Next;
            }
        }
    }
    
    public class Message<T1, T2, T3> : MessageBase
    {
        public void Register(Action<T1, T2, T3> ac)
        {
            RegisterInternal(ac);
        }

        public void UnRegister(Action<T1, T2, T3> ac)
        {
            UnRegisterInternal(ac);
        }

        public void BroadCast(T1 t1, T2 t2, T3 t3)
        {
            if (_delegates.Count == 0)
                return;

            LinkedListNode<Delegate> curNode = _delegates.First;
            while (curNode != null)
            {
                ((Action<T1, T2, T3>)curNode.Value).Invoke(t1, t2, t3);
                curNode = curNode.Next;
            }
        }
    }
    
    public class Message<T1, T2, T3, T4> : MessageBase
    {
        public void Register(Action<T1, T2, T3, T4> ac)
        {
            RegisterInternal(ac);
        }

        public void UnRegister(Action<T1, T2, T3, T4> ac)
        {
            UnRegisterInternal(ac);
        }

        public void BroadCast(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            if (_delegates.Count == 0)
                return;

            LinkedListNode<Delegate> curNode = _delegates.First;
            while (curNode != null)
            {
                ((Action<T1, T2, T3, T4>)curNode.Value).Invoke(t1, t2, t3, t4);
                curNode = curNode.Next;
            }
        }
    }
}