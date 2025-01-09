using System;
using Player;
using UnityEngine;
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

public enum PoolKeys
{
    Money, Chest01, Arrow01, HealthBar, FloatGetItem, FloatDamage
}

public enum FloatText
{
    Open, Get, Use
}

public enum RoomType
{
    NormalRoom, StartRoom, EndRoom, ChestRoom, NpcRoom, BossRoom,
}

public static class EnumManager
{
    public static string RarityToString(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => "Common",
            Rarity.Uncommon => "Uncommon",
            Rarity.Rare => "Rare",
            Rarity.Epic => "Epic",
            Rarity.Legendary => "Legendary",
            _ => rarity.ToString()
        };
    }

    public static Color RarityToColor(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.Common => Color.gray,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            Rarity.Legendary => Color.yellow,
            _ => Color.white
        };
    }
    
    public static string WeaponTypeToString(WeaponType type)
    {
        return type switch
        {
            WeaponType.Sword => "Sword",
            WeaponType.Axe => "Axe",
            WeaponType.Hammer => "Hammer",
            WeaponType.Spear => "Spear",
            WeaponType.Staff => "Staff",
            WeaponType.Wand => "Wand",
            WeaponType.Bow => "Bow",
            _ => type.ToString()
        };
    }        
    
    public static string AttackTypeToString(AttackType type)
    {
        return type switch
        {
            AttackType.Melee => "Melee",
            AttackType.Ranged => "Ranged",
            _ => type.ToString()
        };
    }       
    
    public static string ConsumableTypeToString(ConsumableType type)
    {
        return type switch
        {
            ConsumableType.Food => "Food",
            ConsumableType.Potion => "Potion",
            ConsumableType.Throwable => "Throwable",
            _ => type.ToString()
        };
    }
}