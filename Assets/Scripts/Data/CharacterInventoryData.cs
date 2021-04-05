using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CharacterInventoryData : IComponentData
{
    public Item[] inventory;
}

public enum ItemType{
    none,
    weapon,
    potion,
    food
}
[System.Serializable]
public struct Item{
    public ItemType itemType;
    public Sprite sprite;
    public Weapon weapon;
    public Potion potion;
    public Food food;
}
[System.Serializable]
public struct Weapon{
    public int power;
    public float attackTime;
    public float rechargeTime;
}
[System.Serializable]
public struct Potion{
}
[System.Serializable]
public struct Food{
}
