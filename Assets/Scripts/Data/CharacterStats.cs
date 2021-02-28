using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct CharacterStats : IComponentData
{
    public float health;
    public float recoverTime;
    public int attackMultiplier;
    public Weapon equipedWeapon;
}
public struct Weapon{
    public WeaponType weaponType;
    public float attack;
    public float attackSpeed;
}
public enum WeaponType{
    axe,
    sword
}
