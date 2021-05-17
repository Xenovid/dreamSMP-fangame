using Unity.Entities;
using Unity.Physics;
using UnityEngine;

public class InteractiveBoxCheckerSystem : SystemBase
{
    protected override void OnUpdate()
    {
        /*EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
        if(!playerQuery.IsEmpty){
        
        MovementData playerMovment = playerQuery.GetSingleton<MovementData>();
        OverworldInputData input = GetSingleton<OverworldInputData>(); 
        Entities.ForEach((ref PhysicsCollider collider, in InteractiveBoxCheckerData checkerData) => {
            bool isSet = false;
            if(input.select){
                switch(checkerData.direction){
                    case Direction.up:
                        if(playerMovment.facing == Direction.up){
                            Debug.Log("active");
                            collider.Value.Value.Filter = CollisionFilter.Default;
                            isSet = true;
                        }
                    break;
                    case Direction.down:
                        if(playerMovment.facing == Direction.down){
                            collider.Value.Value.Filter = CollisionFilter.Default;
                            isSet = true;
                        }
                    break;
                    case Direction.right:
                        if(playerMovment.facing == Direction.right){
                            collider.Value.Value.Filter = CollisionFilter.Default;
                            isSet = true;
                        }
                    break;
                    case Direction.left:
                        if(playerMovment.facing == Direction.left){
                            collider.Value.Value.Filter = CollisionFilter.Default;
                            isSet = true;
                        }
                    break;
                }
            }
            if(!isSet){
                collider.Value.Value.Filter = CollisionFilter.Zero;
            }
        }).Schedule();
    }*/
    }
}
