using System;

namespace SFramework.SEvent.Runtime
{
    public class SEvent : SEventBase
    {
        public void Register(Action ac)
        {
            _Register(ac);
        }

        public void UnRegister(Action ac)
        {
            _UnRegister(ac);
        }

        public void BroadCast()
        {
            (_delegate as Action)?.Invoke();
        }
    }
    
    public class SEvent<T1> : SEventBase
    {
        public void Register(Action<T1> ac)
        {
            _Register(ac);
        }

        public void UnRegister(Action<T1> ac)
        {
            _UnRegister(ac);
        }

        public void BroadCast(T1 t1)
        {
            (_delegate as Action<T1>)?.Invoke(t1);
        }
    }
    
    public class SEvent<T1, T2> : SEventBase
    {
        public void Register(Action<T1, T2> ac)
        {
            _Register(ac);
        }

        public void UnRegister(Action<T1, T2> ac)
        {
            _UnRegister(ac);
        }

        public void BroadCast(T1 t1, T2 t2)
        {
            (_delegate as Action<T1, T2>)?.Invoke(t1, t2);
        }
    }
    
    public class SEvent<T1, T2, T3> : SEventBase
    {
        public void Register(Action<T1, T2, T3> ac)
        {
            _Register(ac);
        }

        public void UnRegister(Action<T1, T2, T3> ac)
        {
            _UnRegister(ac);
        }

        public void BroadCast(T1 t1, T2 t2, T3 t3)
        {
            (_delegate as Action<T1, T2, T3>)?.Invoke(t1, t2, t3);
        }
    }
    
    public class SEvent<T1, T2, T3, T4> : SEventBase
    {
        public void Register(Action<T1, T2, T3, T4> ac)
        {
            _Register(ac);
        }

        public void UnRegister(Action<T1, T2, T3, T4> ac)
        {
            _UnRegister(ac);
        }

        public void BroadCast(T1 t1, T2 t2, T3 t3, T4 t4)
        {
            (_delegate as Action<T1, T2, T3, T4>)?.Invoke(t1, t2, t3, t4);
        }
    }
}