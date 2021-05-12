using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TransitionData : IComponentData
{
    public BlobAssetReference<Unity.Physics.Collider> colliderRef;
    public float3 newPosition;
}
