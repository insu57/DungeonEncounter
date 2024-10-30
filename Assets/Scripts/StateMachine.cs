using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class StateMachine<T> where T : class //StateMachine Generic 상태머신 제네릭
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
        if (newState == null) return; //No new state, leave it  새로운 상태가 없으면 그대로

        if(currentState != null) //CurrentState 현재상태 Exit
        {
            previousState = currentState; //PreviousState save 이전상태 저장

            currentState.Exit(ownerCharacter);
        }

        currentState = newState;
        currentState.Enter(ownerCharacter);
        //New State Enter 새로운 상태 Enter
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
