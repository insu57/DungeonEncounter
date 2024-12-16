using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public int moneyAmount { private set; get; }

    public void SetMoneyAmount(int money)
    {
        moneyAmount = money;
    }
}
