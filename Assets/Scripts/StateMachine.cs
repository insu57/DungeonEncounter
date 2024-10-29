using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class StateMachine<T> where T : class //���¸ӽ� ���׸�
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
        if (newState == null) return; //���ο� ���°� ������ �״��

        if(currentState != null) //���� ���� Exit
        {
            previousState = currentState; //�������¿� ����

            currentState.Exit(ownerCharacter);
        }

        currentState = newState;
        currentState.Enter(ownerCharacter);
        //���ο� ���·� Enter
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
