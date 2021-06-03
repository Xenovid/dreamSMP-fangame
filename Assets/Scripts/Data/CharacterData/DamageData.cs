using Unity.Entities;
using UnityEngine;

public struct DamageData : IBufferElementData
{
    public damageType type;
    public float damage;
}
public enum damageType{
    bleeding,
    physical
}