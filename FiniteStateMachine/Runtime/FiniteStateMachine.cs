using System;
using System.Collections.Generic;
using UniWork.Utility.Runtime;

namespace UniWork.FiniteStateMachine.Runtime
{
    public class FiniteStateMachine
    {
        private readonly Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private IState _nowState;

        public void Start(Type type, IStateEnterParams enterParams = null)
        {
            ChangeState(type, enterParams);
        }

        public void Stop()
        {
            _nowState?.OnExist();
            _nowState = null;
        }

        public void Tick()
        {
            _nowState?.OnTick();
        }
        
        public void ChangeState<T>(IStateEnterParams enterParams = null) where T : IState
        {
            ChangeState(typeof(T), enterParams);
        }

        public void ChangeState(Type type, IStateEnterParams enterParams = null)
        {
            if (_states.TryGetValue(type, out IState state) == false)
            {
                DLog.Error("[StateMachine] 不存在该状态: " + type.Name);
                return;
            }
            
            _nowState?.OnExist();
            _nowState = state;
            _nowState?.OnEnter(enterParams);
        }

        public void AddState(IState state)
        {
            Type type = state.GetType();
            if (_states.ContainsKey(type))
            {
                DLog.Error("[StateMachine] 已存在该状态、重复添加: " + type.Name);
                return;
            }
            
            state.Init(this);
            _states.Add(type, state);
        }
    }
}