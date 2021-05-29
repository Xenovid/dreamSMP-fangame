using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
public struct RandomData : IComponentData
{
    public Random Value;
}
