using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
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
        float dt = Time.DeltaTime;

        if(playerQuery.CalculateEntityCount() == 1){
            playerTranslation = playerQuery.GetSingleton<Translation>();
        }
        else{
            Debug.Log("single player not found");
        }
        Entities
        .WithoutBurst()
        .WithNone<PausedTag, BattleData>()
        .ForEach((Animator Animator, ref Translation translation, ref Rotation rotation,  ref BasicEnemyMovementData movementData, ref RandomData random, ref PhysicsVelocity vel, in AnimationData animationData) =>{
            float3 newDirection = new float3(0, 0,0);
            translation.Value = new float3(translation.Value.x, translation.Value.y, 0);
            rotation.Value = quaternion.EulerXYZ(new float3(0,0,0));
            if(math.distance(translation.Value, movementData.followDistance) < 3.0f){
                // enemy should follow the player
                movementData.state = BasicEnemyMovementState.Following;
            }
            switch(movementData.state){
                case BasicEnemyMovementState.RandomDirection:
                    
                    if(movementData.waitTime > 0){
                        movementData.waitTime -= dt;
                    }
                    else{
                        movementData.direction = (Direction)random.Value.NextInt(0, 4);
                        movementData.state = BasicEnemyMovementState.RandomMovement;
                        // how long it will walk for
                        movementData.waitTime = random.Value.NextFloat(1f, 3f);
                    }
                break;
                case BasicEnemyMovementState.RandomMovement:
                    if(movementData.waitTime > 0){
                        movementData.waitTime -= dt;
                        switch(movementData.direction){
                            case Direction.up:
                                newDirection = new float3(0, 1,0);
                            break;
                            case Direction.down:
                                newDirection = new float3(0, -1,0);
                            break;
                            case Direction.right:
                                newDirection = new float3(1, 0, 0);
                            break;
                            case Direction.left:
                                newDirection = new float3(-1, 0,0);
                            break;
                        }
                        
                    }
                    else{
                        movementData.waitTime = random.Value.NextFloat(1f, 3f);
                        movementData.state = BasicEnemyMovementState.RandomDirection;
                    }
                break;
                case BasicEnemyMovementState.Following:
                    if(math.distance(translation.Value, playerTranslation.Value) >= movementData.followDistance){
                        // enemy should follow the player
                        movementData.state = BasicEnemyMovementState.RandomDirection;
                    }
                    else{
                        newDirection = math.normalize(playerTranslation.Value - translation.Value);
                        if(Mathf.Abs(newDirection.x) > Mathf.Abs(newDirection.y))
                        {
                            if(newDirection.x > 0)
                            {
                                movementData.direction = Direction.right;
                            }
                            else
                            {
                                movementData.direction = Direction.left;
                            }
                        }
                        else if(Mathf.Abs(newDirection.y) > Mathf.Abs(newDirection.x))
                        {
                            if(newDirection.y > 0)
                            {
                                movementData.direction = Direction.up;
                            }
                            else
                            {
                                movementData.direction = Direction.down;
                            }
                        }
                    }
                    

                break;
            }
            Animator.SetInteger("direction_state", (int) movementData.direction);
            Animator.SetFloat("moveX", newDirection.x);
            Animator.SetFloat("moveY", newDirection.y);
            vel.Linear = new float3(movementData.speed * newDirection.x, movementData.speed * newDirection.y, 0);
        }).Run();

    }
}
