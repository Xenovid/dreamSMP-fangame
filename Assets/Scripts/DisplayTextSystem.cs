using Unity.Entities;
using System;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Jobs;
using Unity.Collections;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
public class DisplayTextSystem : SystemBase
{   
    private BuildPhysicsWorld buildPhysicsWorld;
    StepPhysicsWorld m_StepPhysicsWorld;
    EntityCommandBuffer tempBuffer = new EntityCommandBuffer(Allocator.Temp);

    protected override void OnCreate()
        {
            m_StepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
        }


    
    protected override void OnUpdate()
    {
        SimulationCallbacks.Callback testTriggerEventsCallback = (ref ISimulation simulation, ref PhysicsWorld world, JobHandle inDeps) =>{
            return new startText{
                TextGroup = GetComponentDataFromEntity<Text>()
            }.Schedule(simulation, ref world, inDeps);
        };

        m_StepPhysicsWorld.EnqueueCallback(SimulationCallbacks.Phase.PostSolveJacobians, testTriggerEventsCallback, Dependency);

    }   
    
    struct startText : ITriggerEventsJob
    {
        public ComponentDataFromEntity<Text> TextGroup;
        public void Execute(TriggerEvent triggerEvent)
        {
            Debug.Log("moo");
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            World tempWorld = World.DefaultGameObjectInjectionWorld;
            var tempBuffer = tempWorld.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            EntityCommandBuffer tempComBuffer = tempBuffer.CreateCommandBuffer();
            if(TextGroup.HasComponent(entityA)){
                
                tempComBuffer.AddComponent(entityA, typeof(Test));
            }
            else if(TextGroup.HasComponent(entityB)){
                tempComBuffer.AddComponent(entityA, typeof(Test));
            }
        }
    }
}


