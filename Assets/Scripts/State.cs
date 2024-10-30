using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> where T : class 
{
    public abstract void Enter(T character);
    public abstract void Execute(T character);
    public abstract void Exit(T character);
}
