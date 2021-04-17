using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Entities;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate(){
        float dT = Time.DeltaTime;

        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        Entities
            .ForEach(
                (ref Translation pos,ref MovementData move) =>
                {
                    move.direction = new float3(input.moveHorizontal, input.moveVertical, 0);
                    float3 dir = math.normalizesafe(move.direction);
                    pos.Value += dir * move.velocity * dT;
                }
            )
            .Schedule();
    }
}
