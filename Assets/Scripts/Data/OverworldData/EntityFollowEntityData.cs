using Unity.Entities;
using Unity.Mathematics;
[GenerateAuthoringComponent]
public struct EntityFollowEntityData : IComponentData
{
    public float3 offset;
    public Entity following;
}
