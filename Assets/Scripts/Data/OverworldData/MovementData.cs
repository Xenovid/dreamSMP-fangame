using Unity.Mathematics;
using Unity.Entities;
[System.Serializable]
[GenerateAuthoringComponent]
public struct MovementData : IComponentData
{
    public MovementMode movementMode;
    public float3 direction;
    public float velocity;

    public Direction facing;
}
public enum MovementMode{
    overworld,
    ui,
    transition
}

