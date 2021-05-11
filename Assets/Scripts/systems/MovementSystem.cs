using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Physics;
using Unity.Entities;
using System;
using Unity.Collections;
public class MovementSystem : SystemBase
{
    public event EventHandler OnTransitionEnd;
    public BattleSystem battleSystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    
    protected override void OnStartRunning(){
        // getting the endsinclation system for a entity component buffer later on
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleStart += MoveToBattlePositions_OnBattleStart;
    }
    protected override void OnUpdate(){
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        float dT = Time.DeltaTime;

        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        Entities
        .ForEach((ref Rotation rot,ref MovementData move, ref PhysicsVelocity vel) =>
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
        }).Schedule();
        bool translationDone = false;
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entity, ref Translation translation, ref TransitionData transitionData) =>
        {
            translation.Value = Vector3.MoveTowards(translation.Value, transitionData.newPosition, 10 * dT);
            if(translation.Value.x == transitionData.newPosition.x && translation.Value.y == transitionData.newPosition.y)
            {
                translationDone = true;
                //ecb.AddComponent(entity, new PhysicsCollider { Value = transitionData.colliderRef });
                ecb.RemoveComponent<TransitionData>(entity);
            }
            else if(HasComponent<Unity.Physics.PhysicsCollider>(entity))
            {
                transitionData.colliderRef = GetComponent<PhysicsCollider>(entity).Value;
                ecb.RemoveComponent<Unity.Physics.PhysicsCollider>(entity);
            }
        }).Run();
        if(translationDone){
            OnTransitionEnd?.Invoke(this, System.EventArgs.Empty);
        }
    }
    private void MoveToBattlePositions_OnBattleStart(System.Object sender, System.EventArgs e){
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityQuery cameraQuery = GetEntityQuery(typeof(Camera));
        Camera camera = cameraQuery.ToComponentArray<Camera>()[0];
        // finds all the players in the party and moves their positions for battle
        int i = 0;
        int playerLength = battleSystem.playerEntities.Count;
        foreach(Entity entity in battleSystem.playerEntities){
            Vector3 tempPos = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth * .1f, ((i + 1) * (camera.pixelHeight / playerLength)) - camera.pixelHeight / (playerLength * 2), 0));
            ecb.AddComponent(entity, new TransitionData{newPosition = new Vector3(tempPos.x,tempPos.y,0)});
            i++;
        }
        i = 0;
        int enemyLength = battleSystem.enemyEntities.Count;
        //finds all the enemies and moves them to battle positions
        foreach(Entity entity in battleSystem.enemyEntities){
            Vector3 tempPos = camera.ScreenToWorldPoint(new Vector3(camera.pixelWidth * .9f, ((i + 1) * (camera.pixelHeight / enemyLength)) - camera.pixelHeight / (enemyLength * 2), 0));
            ecb.AddComponent(entity, new TransitionData{newPosition = new Vector3(tempPos.x,tempPos.y,0)});
            i++;
        }
    }
}
