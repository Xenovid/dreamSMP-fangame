using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class ChestTriggerSystem : SystemBase
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

        if(input.select){
            EntityQuery playerQuery = GetEntityQuery(typeof(PlayerTag), typeof(MovementData));
            MovementData playerMovment = playerQuery.GetSingleton<MovementData>();

            Entity caravan = GetSingletonEntity<CaravanTag>();
            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);

            Entity messageBoard = GetSingletonEntity<OverworldUITag>();
            DynamicBuffer<Text> texts = GetBuffer<Text>(messageBoard);

            foreach(TriggerEvent triggerEvent in triggerEvents)
            {
                
                Entity entityA = triggerEvent.EntityA;
                Entity entityB = triggerEvent.EntityB;

                if (HasComponent<ChestTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
                {
                    Debug.Log("hello");
                    if(GetComponent<InteractiveBoxCheckerData>(entityB).direction == playerMovment.facing){
                        //InputGatheringSystem.currentInput = CurrentInput.ui;
                        if(HasComponent<ChestWeaponData>(entityA)){
                            ChestWeaponData weaponData = GetComponent<ChestWeaponData>(entityA);
                            weaponInventory.Insert(0, new WeaponData{weapon = weaponData.weapon});
                            texts.Add(new Text{text = "you obtained a " + weaponData.weapon.name});
                            EntityManager.RemoveComponent<ChestTag>(entityA);
                            EntityManager.RemoveComponent<ChestWeaponData>(entityA);
                        }
                        else if(HasComponent<ChestArmorData>(entityA)){
                            ChestArmorData armorData = GetComponent<ChestArmorData>(entityA);
                            armorInventory.Insert(0, new ArmorData{armor = armorData.armor});
                            texts.Add(new Text{text = "you obtained a " + armorData.armor.name});
                            EntityManager.RemoveComponent<ChestArmorData>(entityA);
                            EntityManager.RemoveComponent<ChestTag>(entityA);
                        }
                        else if(HasComponent<ChestCharmData>(entityA)){
                            ChestCharmData charmData = GetComponent<ChestCharmData>(entityA);
                            charmInventory.Insert(0, new CharmData{charm = charmData.charm});
                            texts.Add(new Text{text = "you obtained a " + charmData.charm.name});
                            EntityManager.RemoveComponent<ChestTag>(entityA);
                            EntityManager.RemoveComponent<ChestCharmData>(entityA);
                        }
                        else if(HasComponent<ChestItemData>(entityA)){

                        }
                    }  
                }
                else if (HasComponent<ChestTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
                {
                    Debug.Log("hello");
                    //InputGatheringSystem.currentInput = CurrentInput.ui;
                    Animator animator = EntityManager.GetComponentObject<Animator>(entityB);
                    ChestAnimationData animationData = EntityManager.GetComponentObject<ChestAnimationData>(entityB);
                        if(HasComponent<ChestWeaponData>(entityB)){
                            animator.Play(animationData.openAnimationName);
                            ChestWeaponData weaponData = GetComponent<ChestWeaponData>(entityB);
                            weaponInventory.Insert(0, new WeaponData{weapon = weaponData.weapon});
                            texts.Add(new Text{text = "you obtained a " + weaponData.weapon.name});
                            EntityManager.RemoveComponent<ChestWeaponData>(entityB);
                            EntityManager.RemoveComponent<ChestTag>(entityB);
                        }
                        else if(HasComponent<ChestArmorData>(entityB)){
                            animator.Play(animationData.openAnimationName);
                            ChestArmorData armorData = GetComponent<ChestArmorData>(entityB);
                            armorInventory.Insert(0, new ArmorData{armor = armorData.armor});
                            texts.Add(new Text{text = "you obtained a " + armorData.armor.name});
                            EntityManager.RemoveComponent<ChestArmorData>(entityB);
                            EntityManager.RemoveComponent<ChestTag>(entityB);
                        }
                        else if(HasComponent<ChestCharmData>(entityB)){
                            animator.Play(animationData.openAnimationName);
                            ChestCharmData charmData = GetComponent<ChestCharmData>(entityB);
                            charmInventory.Insert(0, new CharmData{charm = charmData.charm});
                            texts.Add(new Text{text = "you obtained a " + charmData.charm.name});
                            EntityManager.RemoveComponent<ChestCharmData>(entityB);
                            EntityManager.RemoveComponent<ChestTag>(entityB);
                        }
                        else if(HasComponent<ChestItemData>(entityB)){

                        }
                }
                m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
            }
        }

    }
}
