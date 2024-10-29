using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class StateMachine<T> where T : class //상태머신 제네릭
{
    private T ownerCharacter;
    private State<T> currentState;
    private State<T> previousState;
    private State<T> globalState;

    public void Setup(T owner, State<T> entryState)
    {
        ownerCharacter = owner;
        currentState = null;
        previousState = null;
        globalState = null;

        ChangeState(entryState);
    }

    public void Execute()
    {
        if(globalState != null)
        {
            globalState.Execute(ownerCharacter);
        }

        if(currentState != null)
        {
            currentState.Execute(ownerCharacter);
        }

    }

    public void ChangeState(State<T> newState)
    {
        if (newState == null) return; //새로운 상태가 없으면 그대로

        if(currentState != null) //현재 상태 Exit
        {
            previousState = currentState; //이전상태에 저장

            currentState.Exit(ownerCharacter);
        }

        currentState = newState;
        currentState.Enter(ownerCharacter);
        //새로운 상태로 Enter
    }

    public void SetGlobalState(State<T> newState)
    {
        globalState = newState;
    }

    public void RevertToPreviousState()
    {
        ChangeState(previousState);
    }
}
