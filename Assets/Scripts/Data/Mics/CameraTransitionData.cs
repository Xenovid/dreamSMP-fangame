using Unity.Entities;
using Unity.Mathematics;
public struct CameraTransitionData : IComponentData
{
    public float3 oldPosition;
    public float3 newPosition;
    public float duration;
    public float timePassed;
}
