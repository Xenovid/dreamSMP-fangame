using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;
public class PolyInteractiveSystem : SystemBase
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

        RequireSingletonForUpdate<CaravanTag>();

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

            Entity messageBoard = GetSingletonEntity<UITag>();
            Text text = GetComponent<Text>(messageBoard);

            Entity entityA = triggerEvent.EntityA;
            Entity entityB = triggerEvent.EntityB;

            if (EntityManager.HasComponent<PolyInteractiveData>(entityA) && HasComponent<InteractiveBoxCheckerData>(entityB))
            {
                PolyInteractiveData interactiveData = EntityManager.GetComponentObject<PolyInteractiveData>(entityA);
                if(!interactiveData.SharedInteractiveData.isTriggered && input.select){
                    interactiveData.SharedInteractiveData.isTriggered = interactiveData.SharedInteractiveData.isSingleUse;
                    EntityManager.SetComponentData(entityA, interactiveData);
                    switch (interactiveData.CurrentTypeId)
                    {
                        case PolyInteractiveData.TypeId.WeaponChestData:

                            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
                            Weapon weapon = WeaponConversionSystem.WeaponInfoToWeapon(interactiveData.WeaponChestData.weapon);
                            SetComponent(messageBoard, new Text{text = "you obtained a " + weapon.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            weaponInventory.Insert(0, new WeaponData{weapon = weapon});
                            Animator weaponanimator = EntityManager.GetComponentObject<Animator>(entityA);
                            weaponanimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.ArmorChestData:
                            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
                            Armor armor = ArmorConversionSystem.ArmorInfoToArmor(interactiveData.ArmorChestData.armor);
                            armorInventory.Insert(0, new ArmorData{armor = armor});
                            SetComponent(messageBoard, new Text{text = "you obtained a " + armor.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            Animator armoranimator = EntityManager.GetComponentObject<Animator>(entityA);
                            armoranimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.CharmChestData:
                            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);
                            Charm charm = CharmConversionSystem.CharmInfoToCharm(interactiveData.CharmChestData.charm);
                            charmInventory.Insert(0, new CharmData{charm = charm});
                            SetComponent(messageBoard, new Text{text = "you obtained a " + charm.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            Animator charmanimator = EntityManager.GetComponentObject<Animator>(entityA);
                            charmanimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.CutsceneInteractiveData:
                            DynamicBuffer<ItemData> items = EntityManager.GetBuffer<ItemData>(caravan);
                            Item item = ItemConversionSystem.ItemInfoToItem(interactiveData.ItemChestData.item);
                            SetComponent(messageBoard, new Text{text = "you obtained a " + item.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            items.Add(new ItemData{item = item});
                            Animator itemanimator = EntityManager.GetComponentObject<Animator>(entityA);
                            itemanimator.Play("ChestOpen");
                            break;
                    }
                    
                }
                else if(!interactiveData.SharedInteractiveData.isTriggered){
                    uISystem.EnableInteractive();
                }
            }
            else if (EntityManager.HasComponent<PolyInteractiveData>(entityB) && HasComponent<InteractiveBoxCheckerData>(entityA))
            {
                PolyInteractiveData interactiveData = EntityManager.GetComponentObject<PolyInteractiveData>(entityB);
                if(!interactiveData.SharedInteractiveData.isTriggered && input.select){
                    interactiveData.SharedInteractiveData.isTriggered = interactiveData.SharedInteractiveData.isSingleUse;
                    EntityManager.SetComponentData(entityB, interactiveData);
                    switch (interactiveData.CurrentTypeId)
                    {
                        case PolyInteractiveData.TypeId.WeaponChestData:
                            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
                            Weapon weapon = WeaponConversionSystem.WeaponInfoToWeapon(interactiveData.WeaponChestData.weapon);
                            SetComponent(messageBoard, new Text{text = "you obtained a " + weapon.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            weaponInventory.Insert(0, new WeaponData{weapon = weapon});
                            Animator weaponanimator = EntityManager.GetComponentObject<Animator>(entityB);
                            weaponanimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.ArmorChestData:
                            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
                            Armor armor = ArmorConversionSystem.ArmorInfoToArmor(interactiveData.ArmorChestData.armor);
                            armorInventory.Insert(0, new ArmorData{armor = armor});
                            SetComponent(messageBoard, new Text{text = "you obtained a " + armor.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            Animator armoranimator = EntityManager.GetComponentObject<Animator>(entityB);
                            armoranimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.CharmChestData:
                            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);
                            Charm charm = CharmConversionSystem.CharmInfoToCharm(interactiveData.CharmChestData.charm);
                            charmInventory.Insert(0, new CharmData{charm = charm});
                            SetComponent(messageBoard, new Text{text = "you obtained a " + charm.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            Animator charmanimator = EntityManager.GetComponentObject<Animator>(entityB);
                            charmanimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.ItemChestData:
                            DynamicBuffer<ItemData> items = EntityManager.GetBuffer<ItemData>(caravan);
                            Item item = ItemConversionSystem.ItemInfoToItem(interactiveData.ItemChestData.item);
                            items.Add(new ItemData{item = item});
                            SetComponent(messageBoard, new Text{text = "you obtained a " + item.name, dialogueSoundName = "default", isEnabled = true, instant = true});
                            inkDisplaySystem.UpdateTextBox();
                            Animator itemanimator = EntityManager.GetComponentObject<Animator>(entityB);
                            itemanimator.Play("ChestOpen");
                            break;
                        case PolyInteractiveData.TypeId.CutsceneInteractiveData:
                            string cutsceneName = interactiveData.CutsceneInteractiveData.cutsceneName;
                            inkDisplaySystem.StartCutScene(cutsceneName);
                            break;
                    }
                    
                }
                else if(!interactiveData.SharedInteractiveData.isTriggered){
                    uISystem.EnableInteractive();
                }
                    
            }
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        }
    }
}
