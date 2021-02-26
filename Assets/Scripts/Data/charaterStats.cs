using Unity.Entities;
using UnityEngine;

public struct charaterStats : IComponentData
{
    public float health;
    public float recoverTime;
    public int attackMultiplier;
}
public struct weapon{
    public WeaponType weaponType;
    public float attack;
    public float attackSpeed;
}
public enum WeaponType{
    axe,
    sword
}
