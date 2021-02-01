using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public class FollowingData : IComponentData
{
    public GameObject gameObjectFollowing;
    public float3 offset;
    public Entity entityToFollow;
}
