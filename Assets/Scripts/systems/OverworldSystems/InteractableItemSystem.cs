using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Physics;
using Unity.Physics.Systems;

public class InteractableItemSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    CollisionWorld collisionWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    InkDisplaySystem inkDisplaySystem;
    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;

        inkDisplaySystem = World.GetExistingSystem<InkDisplaySystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;
        if(input.select){
            EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
            MovementData playerMovment = playerQuery.GetSingleton<MovementData>();

            foreach(TriggerEvent triggerEvent in triggerEvents)
            {
                
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (HasComponent<InteractiveItemData>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
                {
                    if(GetComponent<InteractiveBoxCheckerData>(entityB).direction == playerMovment.facing){
                        CutsceneData text = EntityManager.GetComponentObject<CutsceneData>(entityA);
                        inkDisplaySystem.StartCutScene(text.cutsceneName);
                        InputGatheringSystem.currentInput = CurrentInput.ui;
                    }  
                }
                else if (HasComponent<InteractiveItemData>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
                {
                    if(GetComponent<InteractiveBoxCheckerData>(entityA).direction == playerMovment.facing){
                        CutsceneData text = EntityManager.GetComponentObject<CutsceneData>(entityB);
                        inkDisplaySystem.StartCutScene(text.cutsceneName);
                        InputGatheringSystem.currentInput = CurrentInput.ui;
                    }
                }
            }
        }
        
                }
}
