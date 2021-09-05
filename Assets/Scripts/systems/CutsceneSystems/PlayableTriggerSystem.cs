using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
public class PlayableTriggerSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    PauseSystem pauseSystem;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        physicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        pauseSystem = World.GetOrCreateSystem<PauseSystem>();
    }
    protected override void OnUpdate()
    {
        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        foreach(TriggerEvent triggerEvent in triggerEvents)
        {
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if(EntityManager.HasComponent<PlayableTriggerData>(entityA) && HasComponent<PlayerTag>(entityB)){
                PlayableTriggerData playableTriggerData = EntityManager.GetComponentObject<PlayableTriggerData>(entityA);
                if(!playableTriggerData.isTriggered){
                    playableTriggerData.isTriggered = true;

                    
                    EntityPlayableManager.instance.PlayPlayable(playableTriggerData.index);
                    

                    EntityManager.SetComponentData(entityA, playableTriggerData);
                    pauseSystem.Pause();
                }
            }
            else if(EntityManager.HasComponent<PlayableTriggerData>(entityB) && HasComponent<PlayerTag>(entityA)){
                PlayableTriggerData playableTriggerData = EntityManager.GetComponentObject<PlayableTriggerData>(entityB);
                if(!playableTriggerData.isTriggered){
                    playableTriggerData.isTriggered = true;

                    EntityPlayableManager.instance.PlayPlayable(playableTriggerData.index);
                    

                    EntityManager.SetComponentData(entityB, playableTriggerData);
                    pauseSystem.Pause();
                }
            }
        }
    }
}
