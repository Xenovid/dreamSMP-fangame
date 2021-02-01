using Unity.Entities;
using System;
using Unity.Physics.Systems;
using Unity.Physics;
using UnityEngine;
using UnityEngine.UIElements;
using Unity.Jobs;
using Unity.Collections;

public class DisplayTextSystem : SystemBase
{   
    public BuildPhysicsWorld buildPhysicsWorld;
    public StepPhysicsWorld physicsWorld;
    protected override void OnCreate()
        {
            buildPhysicsWorld = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BuildPhysicsWorld>();
            physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        }


    
    protected override void OnUpdate()
    {   
        Debug.Log("Simulation Type: " + physicsWorld.Simulation.GetType());
        var triggerEvents =  ((Simulation)physicsWorld.Simulation).TriggerEvents;
        ComponentDataFromEntity<Text> TextGroup;
        TextGroup = GetComponentDataFromEntity<Text>();
        foreach(TriggerEvent triggerEvent in triggerEvents){
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;
            if(TextGroup.HasComponent(entityA)){
                Locator.changeText();
            }
            else if(TextGroup.HasComponent(entityB)){
                Locator.changeText();
            }
        }
    }   
    /*
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
    */
}


