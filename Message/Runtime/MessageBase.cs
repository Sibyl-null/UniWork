using System;
using System.Collections.Generic;

namespace UniWork.Message.Runtime
{
    public abstract class MessageBase
    {
        protected readonly LinkedList<Delegate> _delegates = new LinkedList<Delegate>();

        protected void RegisterInternal(Delegate d)
        {
            if (d == null)
                return;

            _delegates.AddLast(d);
        }

        protected void UnRegisterInternal(Delegate d)
        {
            if (d == null)
                return;
            
            _delegates.Remove(d);
        }
    }
}