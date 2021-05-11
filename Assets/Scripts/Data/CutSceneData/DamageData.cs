using Unity.Entities;
using UnityEngine;

[GenerateAuthoringComponent]
public struct DamageData : IBufferElementData
{
    public float damage;
}
