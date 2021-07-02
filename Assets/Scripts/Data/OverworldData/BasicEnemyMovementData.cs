using Unity.Entities;
using UnityEngine;

public struct BasicEnemyMovementData : IComponentData
{
    public Direction direction;
    public float waitTime;
    public BasicEnemyMovementState state;
}

public enum BasicEnemyMovementState{
    RandomDirection,
    RandomMovement,
    Following
}