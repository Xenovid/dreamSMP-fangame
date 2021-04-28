using Unity.Entities;
using Unity.Mathematics;

public struct BeforeBattleData : IComponentData
{
    public BlobAssetReference<Unity.Physics.Collider> colliderRef;
    public float3 previousLocation;
    public bool shouldChangeBack;
}
