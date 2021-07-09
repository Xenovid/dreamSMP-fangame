using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Transforms;
using System;
using System.Collections.Generic;

public class BattleTriggerSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    BattleSystem battleSystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    VisualElement battleUI;

    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();

        battleSystem =  World.GetExistingSystem<BattleSystem>();
    }

    protected override void OnUpdate()
    {
        EntityManager.CompleteAllJobs();

        // finds all the items that caused trigger events
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).CollisionEvents;

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        foreach (CollisionEvent triggerEvent in triggerEvents)
        {
            // the entities from the trigger event
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            //checks if the player hit an entity with battle data on it, and if so triggers a battle
            if (GetComponentDataFromEntity<PlayerTag>().HasComponent(entityA) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityB))
            {
                InputGatheringSystem.currentInput = CurrentInput.ui;
                AudioManager.playSong("tempBattleMusic");
                battleSystem.StartBattle(entityB);
                ecb.RemoveComponent<BattleTriggerData>(entityB);
            }
            else if (GetComponentDataFromEntity<PlayerTag>().HasComponent(entityB) && GetComponentDataFromEntity<BattleTriggerData>().HasComponent(entityA))
            {
                InputGatheringSystem.currentInput = CurrentInput.ui;
                AudioManager.playSong("tempBattleMusic");
                battleSystem.StartBattle(entityA);
                ecb.RemoveComponent<BattleTriggerData>(entityA);
            }
        }
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
    }
}
