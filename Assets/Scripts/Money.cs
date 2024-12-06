using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    public float moneyAmount { private set; get; }

    public void SetMoneyAmount(float money)
    {
        moneyAmount = money;
        Debug.Log("Drop Money: "+moneyAmount);
    }
}
