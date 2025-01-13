using System;
using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class ConsumableDataAssign : ItemDataAssign
{
   [SerializeField] private ConsumableItemData consumableData;
  
   public override IItemData GetItemData()
   {
      return consumableData;
   }

   private void Awake()
   {
      SetParticleArray();
      SetParticleColor(consumableData);
   }
}
