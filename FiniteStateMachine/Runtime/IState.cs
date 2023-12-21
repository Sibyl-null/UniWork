namespace UniWork.FiniteStateMachine.Runtime
{
    public interface IStateEnterParams
    {
    }
    
    public interface IState
    {
        void Init(FiniteStateMachine stateMachine);
        void OnEnter(IStateEnterParams enterParams = null);
        void OnExist();
        void OnTick();
    }
}