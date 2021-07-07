using Unity.Entities;
using UnityEngine;
[GenerateAuthoringComponent]
public struct BasicEnemyMovementData : IComponentData
{
    public float followDistance;
    public float speed;
    public Direction direction;
    public float waitTime;
    public BasicEnemyMovementState state;
}

public enum BasicEnemyMovementState{
    RandomDirection,
    RandomMovement,
    Following
}