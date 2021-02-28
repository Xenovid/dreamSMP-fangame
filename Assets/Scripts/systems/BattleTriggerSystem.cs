using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class BattleTriggerSystem : SystemBase
{
    public StepPhysicsWorld physicsWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate(){
        base.OnCreate();
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();

    }

    protected override void OnUpdate()
    {
        EntityManager.CompleteAllJobs();
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        foreach(TriggerEvent triggerEvent in triggerEvents){
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            
        }
    }
}
