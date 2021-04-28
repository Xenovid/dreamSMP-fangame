using Unity.Entities;
using UnityEngine;

public struct BattleData : IComponentData
{
    public bool isDown;
    public selectables selected;
    [HideInInspector]
    public float maxUseTime;
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