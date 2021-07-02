using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities;
public class BasicEnemyMovementSystem : SystemBase
{
    EntityQuery playerQuery;
    protected override void OnStartRunning()
    {
        playerQuery = GetEntityQuery(typeof(TechnoData), typeof(Translation));
    }
    protected override void OnUpdate()
    {
        Translation playerTranslation = new Translation();
        if(playerQuery.CalculateEntityCount() == 1){
            playerTranslation = playerQuery.GetSingleton<Translation>();
        }
        else{
            Debug.Log("single player not found");
        }
        Entities
        .WithoutBurst()
        .WithNone<PausedTag, BattleData>()
        .ForEach((Animator Animator, ref Translation translation,  ref BasicEnemyMovementData movementData, in AnimationData animationData) =>{
            if(math.distance(translation.Value, playerTranslation.Value) < 2.0f){
                // enemy should follow the player
                movementData.state = BasicEnemyMovementState.Following;
            }
            switch(movementData.state){
                case BasicEnemyMovementState.RandomDirection:
                    
                break;
            }
        }).Run();
    }
}
