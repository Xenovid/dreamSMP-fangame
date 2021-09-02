using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TransitionData : IComponentData
{
    public float3 oldPosition;
    public float3 newPosition;
    public float timePassed;
    public float duration;
}
