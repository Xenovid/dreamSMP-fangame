using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
public class RandomDataAuthouring : MonoBehaviour{
}
[GenerateAuthoringComponent]
public struct RandomData : IComponentData
{
    public Random Value;
}

public class RandomConversionSystem : GameObjectConversionSystem
{
    int i = 0;
    protected override void OnUpdate()
    {
        Entities.ForEach((RandomDataAuthouring randomData) => {
            Random random = new Random();
            random.InitState((uint)i);
            i++;
        });
    }
}
