using System;

namespace UniWork.Message.Runtime
{
    public abstract class MessageBase
    {
        protected Delegate _delegate;

        protected void _Register(Delegate d)
        {
            _delegate = Delegate.Combine(_delegate, d);
        }

        protected void _UnRegister(Delegate d)
        {
            _delegate = Delegate.Remove(_delegate, d);
        }
    }
}