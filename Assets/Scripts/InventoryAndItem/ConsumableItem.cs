using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class ConsumableItem : MonoBehaviour
{
    [SerializeField] private ConsumableItemData data;
    public ConsumableItemData Data => data;
    
}
