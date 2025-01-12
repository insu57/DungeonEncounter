using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IItemData
{
    //ItemDataInterface
    public ItemTypes ItemType { get; }

    public string GetName();
    public string GetDescription();
    public Rarity GetRarity();
    public Sprite GetIcon();
    public ItemEffect[] GetEffects();
    
}
