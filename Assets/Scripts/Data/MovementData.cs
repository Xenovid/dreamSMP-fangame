using Unity.Mathematics;
using UnityEngine;
using Unity.Entities;

[GenerateAuthoringComponent]
struct MovementData : IComponentData
{
    public float3 direction;
    public float velocity;

}

