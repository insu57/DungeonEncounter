using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class GetItemType : MonoBehaviour
{
    [SerializeField] private ItemTypes itemType;
    public ItemTypes ItemType => itemType;
}
