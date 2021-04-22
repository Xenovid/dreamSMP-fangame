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

    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
        collisionWorld = physicsWorldSystem.PhysicsWorld.CollisionWorld;
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        if (input.select)
        {
            Entities
            .WithoutBurst()
            .WithAll<PlayerTag>()
            .ForEach((in Entity ent, in Translation playerTranslation,in MovementData move) =>
            {   
                foreach(TriggerEvent triggerEvent in triggerEvents)
                   {
                        
                    Entity entityA = triggerEvent.EntityA;
                    Entity entityB = triggerEvent.EntityB;
                        

                    if(entityA == ent || entityB == ent)
                    {
                        Entity interactiveEntity = new Entity();
                        bool changed = false;

                        if (HasComponent<InteractiveItemData>(entityA))
                        {
                            interactiveEntity = entityA;
                            changed = true;
                        }
                        else if (HasComponent<InteractiveItemData>(entityB))
                        {
                            interactiveEntity = entityB;
                            changed = true;
                        }
                        if (changed)
                        {
                            Translation interactiveTranslation = GetComponent<Translation>(interactiveEntity);
                            DynamicBuffer<Text> text = GetBuffer<Text>(interactiveEntity);
                            bool facingItemDirection = false;
                            switch (move.facing)
                            {
                                case Direction.up:
                                    facingItemDirection = playerTranslation.Value.y < interactiveTranslation.Value.y;
                                    break;
                                case Direction.down:
                                    facingItemDirection = playerTranslation.Value.y > interactiveTranslation.Value.y;
                                    break;
                                case Direction.left:
                                    facingItemDirection = playerTranslation.Value.x > interactiveTranslation.Value.x;
                                    break;
                                case Direction.right:
                                    facingItemDirection = playerTranslation.Value.x < interactiveTranslation.Value.x;
                                    break;
                            }
                            if (facingItemDirection)
                            {
                                ecb.AddComponent<TextBoxData>(interactiveEntity);

                                InputGatheringSystem.currentInput = CurrentInput.ui;
                            }

                        }
                    }
                }
            }).Run();
        }
    }

}
