using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Entities;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate(){
        float dT = Time.DeltaTime;

        Entities
            .WithNone<CutsceneData>()
            .ForEach(
                (ref Translation pos,in MovementData move) =>
                {
                    float3 dir = math.normalizesafe( move.direction);
                    pos.Value += dir * move.velocity * dT;
                }
            )
            .Run();
    }
}
