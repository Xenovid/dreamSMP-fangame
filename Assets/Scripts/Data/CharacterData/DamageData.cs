using Unity.Entities;
using UnityEngine;

public struct DamageData : IBufferElementData
{
    public StatusEffect statusEffect;
    public damageType type;
    public float damage;
}
public enum damageType{
    bleeding,
    physical
}
[System.Serializable]
public enum StatusEffect{
    none,
    bleeding
}