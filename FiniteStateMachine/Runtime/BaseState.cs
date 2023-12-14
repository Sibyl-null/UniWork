namespace UniWork.FiniteStateMachine.Runtime
{
    public interface IStateEnterParams
    {
    }
    
    public abstract class BaseState
    {
        protected FiniteStateMachine _stateMachine;

        public void Init(FiniteStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public abstract void OnEnter(IStateEnterParams enterParams = null);
        public abstract void OnExist();
        public abstract void OnTick();
    }
}