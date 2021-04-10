using Unity.Entities;
using UnityEngine;

public struct BattleData : IComponentData
{
    public selectables selected;
    public float useTime;
    public int targetingId;
    public float damage;

    public ItemType equiptype;
    public ItemType itemType;
}
public enum weaponType{
    fist,
    axe,
    sword
}