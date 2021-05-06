using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CharacterInventoryData : IComponentData
{
    public Item[] inventory;
    public Weapon[] weapons;
    public Armor[] armors;
    public Charm[] charms;
    public Skill[] skills;
    public Skill[] equipedSkills;
    public Weapon equipedWeapon;
    public Armor equipedArmor;
    public Charm equipedCharm;
}

public enum ItemType{
    none,
    axe,
    sword,
    potion,
    food
}
[System.Serializable]
public struct Item{
    public ItemType itemType;
    public Potion potion;
    public Food food;
    public string name;
    public string description;
    public float useTime;
}
[System.Serializable]
public struct Weapon{
    public int power;
    public float attackTime;
    public float rechargeTime;
    public string name;
    public string description;
    public float useTime;
}
[System.Serializable]
public struct Charm{
    //add features
    public string name;
    public string description;
}
[System.Serializable]
public struct Armor{
    public string name;
    public int defense;
    public string description;
}
[System.Serializable]
public struct Potion{
}
[System.Serializable]
public struct Food{
}
[System.Serializable]
public struct Skill{
    public string name;
    public string description;
    public int damageBoost;
    public int cost;
}
