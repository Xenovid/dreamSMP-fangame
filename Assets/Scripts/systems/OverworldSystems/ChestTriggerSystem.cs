using Unity.Entities;
using Unity.Transforms;
using Unity.Physics;
using Unity.Physics.Systems;

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
                    if(GetComponent<InteractiveBoxCheckerData>(entityB).direction == playerMovment.facing){
                        //InputGatheringSystem.currentInput = CurrentInput.ui;
                        if(HasComponent<ChestWeaponData>(entityA)){
                            ChestWeaponData weaponData = GetComponent<ChestWeaponData>(entityA);
                            weaponInventory.Insert(0, new WeaponData{weapon = weaponData.weapon});
                            texts.Add(new Text{text = "you obtained a" + weaponData.weapon.name});
                            ecb.RemoveComponent<ChestTag>(entityA);
                            ecb.RemoveComponent<ChestWeaponData>(entityA);
                        }
                        else if(HasComponent<ChestArmorData>(entityA)){
                            ChestArmorData armorData = GetComponent<ChestArmorData>(entityA);
                            armorInventory.Insert(0, new ArmorData{armor = armorData.armor});
                            texts.Add(new Text{text = "you obtained a" + armorData.armor.name});
                            ecb.RemoveComponent<ChestArmorData>(entityA);
                            ecb.RemoveComponent<ChestTag>(entityA);
                        }
                        else if(HasComponent<ChestCharmData>(entityA)){
                            ChestCharmData charmData = GetComponent<ChestCharmData>(entityA);
                            charmInventory.Insert(0, new CharmData{charm = charmData.charm});
                            texts.Add(new Text{text = "you obtained a" + charmData.charm.name});
                            ecb.RemoveComponent<ChestTag>(entityA);
                            ecb.RemoveComponent<ChestCharmData>(entityA);
                        }
                        else if(HasComponent<ChestItemData>(entityA)){

                        }
                    }  
                }
                else if (HasComponent<ChestTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
                {
                    //InputGatheringSystem.currentInput = CurrentInput.ui;
                        if(HasComponent<ChestWeaponData>(entityB)){
                            ChestWeaponData weaponData = GetComponent<ChestWeaponData>(entityB);
                            weaponInventory.Insert(0, new WeaponData{weapon = weaponData.weapon});
                            texts.Add(new Text{text = "you obtained a" + weaponData.weapon.name});
                            ecb.RemoveComponent<ChestWeaponData>(entityB);
                            ecb.RemoveComponent<ChestTag>(entityB);
                        }
                        else if(HasComponent<ChestArmorData>(entityB)){
                            ChestArmorData armorData = GetComponent<ChestArmorData>(entityB);
                            armorInventory.Insert(0, new ArmorData{armor = armorData.armor});
                            texts.Add(new Text{text = "you obtained a" + armorData.armor.name});
                            ecb.RemoveComponent<ChestArmorData>(entityB);
                            ecb.RemoveComponent<ChestTag>(entityB);
                        }
                        else if(HasComponent<ChestCharmData>(entityB)){
                            ChestCharmData charmData = GetComponent<ChestCharmData>(entityB);
                            charmInventory.Insert(0, new CharmData{charm = charmData.charm});
                            texts.Add(new Text{text = "you obtained a" + charmData.charm.name});
                            ecb.RemoveComponent<ChestCharmData>(entityB);
                            ecb.RemoveComponent<ChestTag>(entityB);
                        }
                        else if(HasComponent<ChestItemData>(entityB)){

                        }
                }
                m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
            }
        }

    }
}
