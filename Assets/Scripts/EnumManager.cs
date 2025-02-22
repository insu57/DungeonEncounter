using System;
using Player;
using UnityEngine;

[Serializable]
public class ItemEffect
{
    public PlayerStatTypes effectStat;
    public CalculateType effectCalculate;
    public float effectValue;
    public string effectDescription;
    public float effectDuration;
    public bool isTickBased;
    public float tickSecond;
    public EffectType effectType;
    public bool useOtherStat;
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
    Money, Chest01, Arrow01, HealthBar, FloatDamage, SkeletonNormal, SkeletonArcher, NormalSlime
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
            Rarity.Common => Color.white,
            Rarity.Uncommon => Color.green,
            Rarity.Rare => Color.blue,
            Rarity.Epic => Color.magenta,
            Rarity.Legendary => Color.yellow,
            _ => Color.gray
        };
    }

    public static Rarity StringToRarity(string rarityString)
    {
        return rarityString switch
        {
            "Common" => Rarity.Common,
            "Uncommon" => Rarity.Uncommon,
            "Rare" => Rarity.Rare,
            "Epic" => Rarity.Epic,
            "Legendary" => Rarity.Legendary,
            _ => Rarity.Common
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

    public static WeaponType StringToWeaponType(string type)
    {
        return type switch
        {
            "Sword" => WeaponType.Sword,
            "Axe" => WeaponType.Axe,
            "Hammer" => WeaponType.Hammer,
            "Spear" => WeaponType.Spear,
            "Staff" => WeaponType.Staff,
            "Wand" => WeaponType.Wand,
            "Bow" => WeaponType.Bow,
            _ => WeaponType.Sword
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

    public static AttackType StringToAttackType(String type)
    {
        return type switch
        {
            "Melee" => AttackType.Melee,
            "Ranged" => AttackType.Ranged,
            _ => AttackType.Melee
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