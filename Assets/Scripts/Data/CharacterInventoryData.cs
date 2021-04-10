using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public class CharacterInventoryData : IComponentData
{
    public Item[] inventory;
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
    public Weapon weapon;
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
}
[System.Serializable]
public struct Potion{
}
[System.Serializable]
public struct Food{
}
