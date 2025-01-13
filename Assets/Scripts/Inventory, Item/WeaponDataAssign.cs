using System.Collections;
using System.Collections.Generic;
using Scriptable_Objects;
using UnityEngine;

public class WeaponDataAssign : ItemDataAssign
{
   [SerializeField] private PlayerWeaponData weaponData;

   public override IItemData GetItemData()
   {
      return weaponData;
   }
   private void Awake()
   {
      SetParticleArray();
      SetParticleColor(weaponData);
   }
}
