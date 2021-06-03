using Unity.Entities;
using UnityEngine;

public struct DamageData : IBufferElementData
{
    public damageColor color;
    public float damage;
}
public enum damageColor{
    red,
    white
}