using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CharacterInventoryData : IComponentData
{
    public Item[] inventory;
    public Weapon[] weapons;
    public Armor[] armors;
    public Charm[] charms;
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
    public Sprite sprite;
    public Potion potion;
    public Food food;
    public string description;
    public float useTime;
}
[System.Serializable]
public struct Weapon{
    public int power;
    public float attackTime;
    public float rechargeTime;
    public string description;
    public float useTime;
}
public struct Charm{
    //add features
    public string description;
}
public struct Armor{
    public int defense;
    public string description;
}
[System.Serializable]
public struct Potion{
}
[System.Serializable]
public struct Food{
}
