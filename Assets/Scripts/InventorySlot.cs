using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public int Index { get; private set; }
    [SerializeField] private Button button;

    public void Initialize(int index)
    { 
        Index = index;
    }
}
