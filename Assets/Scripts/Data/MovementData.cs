using Unity.Mathematics;
using Unity.Entities;

[GenerateAuthoringComponent]
struct MovementData : IComponentData
{
    public float3 direction;
    public float velocity;
}

