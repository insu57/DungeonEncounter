using System;
using Player;
using UnityEngine.Serialization;

[Serializable]
public class ItemEffect
{
    public PlayerStatTypes effectStat;
    public CalculateType effectCalculate;
    public float effectAmount;
    public string effectDescription;
    public float effectDuration;
    public bool isTickBased;
    public float tickSecond;
    public EffectType effectType;
    //즉발.지연 추가 필요
}
public enum JobTypes
{
    Warrior, Magician,
}

public enum ItemTypes
{
    Weapon, Equipment, Consumable, Money, Chest
}

public enum ItemLayers
{
    Weapon = 9, Equipment = 10, Consumable = 11, Money = 12, Chest = 13
}

public enum WeaponType
{
    Sword, Axe, Hammer, Spear, Wand, Staff, Bow
}

public enum AttackType
{
    Melee, Ranged
}

public enum ConsumableType
{
    Potion, Food, Throwable
}

public enum ChestType
{
    Weapon, Equipment, Consumable, Money,
}

public enum CalculateType
{
    Plus, Multiply
}

public enum EffectType
{
    Instant, Temporary, Permanent
}

public enum Rarity
{
    Common, Uncommon, Rare, Epic, Legendary,
}

