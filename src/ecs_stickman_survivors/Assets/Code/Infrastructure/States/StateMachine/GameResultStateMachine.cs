using Code.Infrastructure.States.Factory;
using Code.Infrastructure.States.StateInfrastructure;
using RSG;
using Zenject;

namespace Code.Infrastructure.States.StateMachine
{
    public class GameResultStateMachine : IGameResultStateMachine, ITickable
    {
        private IExitableState _activeState;
        private readonly IStateFactory _stateFactory;

        public GameResultStateMachine(IStateFactory stateFactory)
        {
            _stateFactory = stateFactory;
        }

        public void Tick()
        {
            if (_activeState is IUpdateable updateableState)
                updateableState.Update();
        }

        public void Enter<TState>() where TState : class, IState =>
            RequestEnter<TState>()
                .Done();

        public void Enter<TState, TPayload>(TPayload payload) where TState : class, IPayloadState<TPayload> =>
            RequestEnter<TState, TPayload>(payload)
                .Done();

        private IPromise<TState> RequestEnter<TState>() where TState : class, IState =>
            RequestChangeState<TState>()
                .Then(EnterState);

        private IPromise<TState> RequestEnter<TState, TPayload>(TPayload payload)
            where TState : class, IPayloadState<TPayload> =>
            RequestChangeState<TState>()
                .Then(state => EnterPayloadState(state, payload));

        private TState EnterState<TState>(TState state) where TState : class, IState
        {
            _activeState = state;

            state.Enter();
            return state;
        }

        private TState EnterPayloadState<TState, TPayload>(TState state, TPayload payload)
            where TState : class, IPayloadState<TPayload>
        {
            _activeState = state;

            state.Enter(payload);
            return state;
        }

        private IPromise<TState> RequestChangeState<TState>() where TState : class, IExitableState
        {
            if (_activeState != null)
            {
                return _activeState
                    .BeginExit()
                    .Then(_activeState.EndExit)
                    .Then(ChangeState<TState>);
            }

            return ChangeState<TState>();
        }


        private IPromise<TState> ChangeState<TState>() where TState : class, IExitableState
        {
            TState state = _stateFactory.GetState<TState>();

            return Promise<TState>.Resolved(state);
        }
    }
}