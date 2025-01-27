using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "ItemPrice", menuName = "ItemPrice")]
public class ItemPrice : ScriptableObject
{
    [Serializable]
    public class ItemPriceData
    {
        public int commonPrice;
        public int uncommonPrice;
        public int rarePrice;
        public int epicPrice;
        public int legendaryPrice;
    }
    
    public ItemPriceData consumablePrice = new ItemPriceData();
    public ItemPriceData weaponEquipmentPrice = new ItemPriceData();
    

}
