using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Entities;

public class MovementSystem : SystemBase
{
    protected override void OnUpdate(){
        float dT = Time.DeltaTime;

        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        Entities
            .ForEach(
                (ref Rotation rot,ref MovementData move, ref PhysicsVelocity vel) =>
                {
                    move.direction = new float3(input.moveHorizontal, input.moveVertical, 0);
                    if(Mathf.Abs(move.direction.x) > Mathf.Abs(move.direction.y))
                    {
                        if(move.direction.x > 0)
                        {
                            move.facing = Direction.right;
                        }
                        else
                        {
                            move.facing = Direction.left;
                        }
                    }
                    else if(Mathf.Abs(move.direction.y) > Mathf.Abs(move.direction.x))
                    {
                        if(move.direction.y > 0)
                        {
                            move.facing = Direction.up;
                        }
                        else
                        {
                            move.facing = Direction.down;
                        }
                    }
                    float3 dir = math.normalizesafe(move.direction);
                    
                    vel.Angular = 0;
                    rot.Value = quaternion.EulerZXY(new float3(0,0,0));
                    float3 sprintBoost = input.sprint ? dir * move.velocity * .9f : 0;
                    vel.Linear = dir * move.velocity + sprintBoost;
                    //pos.Value += dir * move.velocity * dT;
                }
            )
            .Schedule();
    }
}
