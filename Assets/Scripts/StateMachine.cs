
public  class StateMachine<T> where T : class //StateMachine Generic 상태머신 제네릭
{
    private T _ownerCharacter;
    private State<T> _currentState;
    private State<T> _previousState;
    private State<T> _globalState;

    public void Setup(T owner, State<T> entryState)
    {
        _ownerCharacter = owner;
        _currentState = null;
        _previousState = null;
        _globalState = null;

        ChangeState(entryState);
    }

    public void Execute()
    {
        if(_globalState != null)
        {
            _globalState.Execute(_ownerCharacter);
        }

        if(_currentState != null)
        {
            _currentState.Execute(_ownerCharacter);
        }

    }

    public void ChangeState(State<T> newState)
    {
        if (newState == null) return; //No new state, leave it  새로운 상태가 없으면 그대로

        if(_currentState != null) //CurrentState 현재상태 Exit
        {
            _previousState = _currentState; //PreviousState save 이전상태 저장

            _currentState.Exit(_ownerCharacter);
        }

        _currentState = newState;
        _currentState.Enter(_ownerCharacter);
        //New State Enter 새로운 상태 Enter
    }

    public void SetGlobalState(State<T> newState)
    {
        _globalState = newState;
    }

    public void RevertToPreviousState()
    {
        ChangeState(_previousState);
    }
}
