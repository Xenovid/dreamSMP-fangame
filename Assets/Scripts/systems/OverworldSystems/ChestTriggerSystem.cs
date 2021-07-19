using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;

public class ChestTriggerSystem : SystemBase
{
    StepPhysicsWorld physicsWorld;
    UISystem uISystem;
    InkDisplaySystem inkDisplaySystem;
    InventorySystem inventorySystem;
    CollisionWorld collisionWorld;
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnCreate()
    {
        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        uISystem = World.GetOrCreateSystem<UISystem>();
        inkDisplaySystem = World.GetOrCreateSystem<InkDisplaySystem>();
        inventorySystem = World.GetOrCreateSystem<InventorySystem>();
        physicsWorld = World.GetExistingSystem<StepPhysicsWorld>();
        var physicsWorldSystem = World.GetExistingSystem<Unity.Physics.Systems.BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        EntityQuery uiInputQuery = GetEntityQuery(typeof(OverworldInputData));
        OverworldInputData input = uiInputQuery.GetSingleton<OverworldInputData>();

        var triggerEvents = ((Simulation)physicsWorld.Simulation).TriggerEvents;

        foreach(TriggerEvent triggerEvent in triggerEvents)
        {
            Entity caravan = GetSingletonEntity<CaravanTag>();
            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);

            DynamicBuffer<ItemData> items = EntityManager.GetBuffer<ItemData>(GetSingletonEntity<PlayerTag>());

            Entity messageBoard = GetSingletonEntity<UITag>();
            Text text = GetComponent<Text>(messageBoard);
            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (HasComponent<ChestTag>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
            {
                bool isChestOpen = GetComponent<ChestTag>(entityA).isOpen;
                if(input.select && !isChestOpen){
                    //InputGatheringSystem.currentInput = CurrentInput.ui;
                    if(HasComponent<ChestWeaponData>(entityA)){
                        ChestWeaponData weaponData = GetComponent<ChestWeaponData>(entityA);
                        SetComponent<ChestTag>(entityA, new ChestTag{isOpen = true});
                        SetComponent(messageBoard, new Text{text = "you obtained a " + weaponData.weapon.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                        inkDisplaySystem.UpdateTextBox();
                        inventorySystem.AddWeapon("weaponData.weapon.name");
                    }
                    else if(HasComponent<ChestArmorData>(entityA)){
                        ChestArmorData armorData = GetComponent<ChestArmorData>(entityA);
                        SetComponent<ChestTag>(entityA, new ChestTag{isOpen = true});
                        armorInventory.Insert(0, new ArmorData{armor = armorData.armor});
                        //texts.Add(new Text{text = "you obtained a " + armorData.armor.name, instant = true});
                    }
                    else if(HasComponent<ChestCharmData>(entityA)){
                        ChestCharmData charmData = GetComponent<ChestCharmData>(entityA);
                        SetComponent<ChestTag>(entityA, new ChestTag{isOpen = true});
                        charmInventory.Insert(0, new CharmData{charm = charmData.charm});
                        //texts.Add(new Text{text = "you obtained a " + charmData.charm.name, instant = true});
                    }
                    else if(HasComponent<ChestItemData>(entityA)){
                        if(items.Length < 10){
                            SetComponent<ChestTag>(entityA, new ChestTag{isOpen = true});
                            ChestItemData itemData = GetComponent<ChestItemData>(entityA);
                            inventorySystem.AddItem(itemData.item.name.ToString());
                            SetComponent(messageBoard, new Text{text = "you obtained a " + itemData.item.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            inventorySystem.AddWeapon(itemData.item.name.ToString());
                        }
                        else{
                            SetComponent(messageBoard, new Text{text = "you don't have enough room for items", dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                        }
                    }
                }
                else if(!isChestOpen){
                    // activate the visual indicator to let the player know they can interact with something
                    uISystem.EnableInteractive();
                }      
            }
            else if (HasComponent<ChestTag>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
            {
                bool isChestOpen = GetComponent<ChestTag>(entityB).isOpen;
                if(input.select && !isChestOpen){   
                    //InputGatheringSystem.currentInput = CurrentInput.ui;
                    Animator animator = EntityManager.GetComponentObject<Animator>(entityB);
                    ChestAnimationData animationData = EntityManager.GetComponentObject<ChestAnimationData>(entityB);
                    if(HasComponent<ChestWeaponData>(entityB)){
                        animator.Play(animationData.openAnimationName);
                        SetComponent<ChestTag>(entityB, new ChestTag{isOpen = true});
                        ChestWeaponData weaponData = GetComponent<ChestWeaponData>(entityB);
                        weaponInventory.Insert(0, new WeaponData{weapon = weaponData.weapon});
                        SetComponent(messageBoard, new Text{text = "you obtained a " + weaponData.weapon.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                        inkDisplaySystem.UpdateTextBox();
                    }
                    else if(HasComponent<ChestArmorData>(entityB)){
                        animator.Play(animationData.openAnimationName);
                        SetComponent<ChestTag>(entityB, new ChestTag{isOpen = true});
                        ChestArmorData armorData = GetComponent<ChestArmorData>(entityB);
                        armorInventory.Insert(0, new ArmorData{armor = armorData.armor});
                        //texts.Add(new Text{text = "you obtained a " + armorData.armor.name, instant = true});
                    }
                    else if(HasComponent<ChestCharmData>(entityB)){
                        animator.Play(animationData.openAnimationName);
                        SetComponent<ChestTag>(entityB, new ChestTag{isOpen = true});
                        ChestCharmData charmData = GetComponent<ChestCharmData>(entityB);
                        charmInventory.Insert(0, new CharmData{charm = charmData.charm});
                        //texts.Add(new Text{text = "you obtained a " + charmData.charm.name, instant = true});
                    }
                    else if(HasComponent<ChestItemData>(entityB)){
                        
                        if(items.Length < 10){
                            animator.Play(animationData.openAnimationName);
                            SetComponent<ChestTag>(entityB, new ChestTag{isOpen = true});
                            ChestItemData itemData = GetComponent<ChestItemData>(entityB);
                            SetComponent(messageBoard, new Text{text = "you obtained a " + itemData.item.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            inventorySystem.AddWeapon(itemData.item.name.ToString());
                        }
                        else{
                            //texts.Add(new Text{text = "you don't have enough room to pick up this item", instant = true});
                        }
                    }
                }
                else if(!isChestOpen){
                    // activate the visual indicator to let the player know they can interact with something
                    uISystem.EnableInteractive();
                }
                    
            }
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }

    }
}
