using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State<T> where T : class 
{
    public abstract void Enter(T character);
    //���� ���� �� 1ȸ ȣ��
    public abstract void Execute(T character);
    //�� ������ ȣ��
    public abstract void Exit(T character);
    //���� �� ȣ��
}
