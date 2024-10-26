using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> where T : class 
{
    public abstract void Enter(T character);
    //상태 시작 시 1회 호출
    public abstract void Execute(T character);
    //매 프레임 호출
    public abstract void Exit(T character);
    //종료 시 호출
}
