using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class GetItemData : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    public ItemData ItemData => itemData;
    public ItemTypes ItemType => itemData.ItemType;
}
