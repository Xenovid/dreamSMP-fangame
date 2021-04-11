using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Collections;

public class BattleMenuSystem : SystemBase
{
    public selectables currentSelection = selectables.attack;
    public VisualElement battleUI;
    UIDocument UIDoc;
    public int test = 0;

    //private bool isBattleMenuOn = false;
    private menuType currentMenu;
    private int currentCharacterSelected;
    private int currentEnemySelected;

    public int playerNumber;
    public bool hasMoved;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnStartRunning(){
        currentMenu = menuType.battleMenu;
        base.OnStartRunning();

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

        Entities
        .WithoutBurst()
        .WithAll<BattleUITag>()
        .ForEach((UIDocument UI) =>{
                UIDoc = UI;
         }).Run();

        int templength = 0;
        Entities
        .ForEach((DynamicBuffer<PlayerPartyData> party) =>{
            templength = party.Length;
        }).Run();

        playerNumber = templength;
    }

    protected override void OnUpdate()
    {
        EntityManager.CompleteAllJobs();
        float deltaTime = Time.DeltaTime;

        EntityQuery BattleManagerGroup = GetEntityQuery(typeof(BattleManagerTag));
        NativeArray<Entity> battleManagers = BattleManagerGroup.ToEntityArray(Allocator.Temp);
        
        EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
        NativeArray<Entity> enemyUiSelection = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);

        DynamicBuffer<EnemyBattleData> EnemyIds = new DynamicBuffer<EnemyBattleData>();

        EntityQuery battleCharacters = GetEntityQuery(typeof(CharacterStats), typeof(BattleData));

        bool isBattling = false;
        
        foreach(Entity entity in battleManagers){
            EnemyIds = GetBuffer<EnemyBattleData>(entity);
            isBattling = true;
        }
        if(!isBattling){
            
        }

        UIInputData input = new UIInputData();
        Entities
        .WithoutBurst()
        .WithAll<BattleUITag>()
        .ForEach((UIInputData temp) =>{
                input = temp;
         }).Run();

        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

        Color black = Color.black;
        Color grey = Color.grey;
        var rootVisualElement = UIDoc.rootVisualElement;
        if(rootVisualElement == null){
            Debug.Log("didn't find root visual element");
        }
        else{
            battleUI = rootVisualElement.Q<VisualElement>("BattleUI");
            VisualElement itemDesc = rootVisualElement.Q<VisualElement>("Itemdesc");
            Label itemTextBox = rootVisualElement.Q<Label>("itemTextBox");

            VisualElement enemySelector = rootVisualElement.Q<VisualElement>("EnemySelector");
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((CharacterInventoryData inventory, AnimationData animation, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStat, in Entity entity) =>{
                battleData.selected = selectables.none;
                if(!isBattling){
                    //To Do: should display victory screen
                }
                else if(selectorUI.isSelectable){
                    VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");
                    useBar.style.width = 0;
                    //isBattleMenuOn = true;
                    switch (currentMenu){
                        case menuType.battleMenu:
                            battleData.itemType = inventory.inventory[selectorUI.currentItem].itemType;
                            battleUI.visible = true;
                            itemDesc.visible = true;
                            enemySelector.visible = false;
                            if(selectorUI.isHovered && currentCharacterSelected == entityInQueryIndex && !hasMoved){
                                selectorUI.UI.AddToClassList("hovering");
                                if(input.goselected){
                                    AudioManager.playSound("menuchange");
                                    selectorUI.isHovered = false;
                                    selectorUI.isSelected = true;

                                    selectorUI.UI.RemoveFromClassList("hovering");
                                    selectorUI.UI.AddToClassList("selected");
                                }
                                else if(input.moveleft){
                                    hasMoved = true;
                                    AudioManager.playSound("menuchange");
                                    currentCharacterSelected--;

                                    selectorUI.UI.RemoveFromClassList("hovering");
                                }
                                else if(input.moveright){
                                    hasMoved = true;
                                    AudioManager.playSound("menuchange");
                                    currentCharacterSelected++;

                                    selectorUI.UI.RemoveFromClassList("hovering");
                                }
                                if(currentCharacterSelected < 0){
                                    currentCharacterSelected = playerNumber - 1;
                                }
                                else if(currentCharacterSelected >= playerNumber){
                                    currentCharacterSelected = 0;
                                }
                            }
                            else if(selectorUI.isSelected){
                                itemTextBox.text = inventory.inventory[selectorUI.currentItem].description;
                                if(input.goback){
                                    AudioManager.playSound("menuchange");
                                    selectorUI.isSelected = false;
                                    selectorUI.isHovered = true;

                                    selectorUI.UI.RemoveFromClassList("selected");
                                    selectorUI.UI.AddToClassList("hovering");
                                }
                                else if(input.goselected){
                                    //use item and start a delay
                                    Item currentItem = inventory.inventory[selectorUI.currentItem];
                                    switch(currentItem.itemType){
                                        case ItemType.sword:
                                            if(currentItem.weapon.rechargeTime <= 0){
                                                currentMenu = menuType.attackMenu;
                                            }
                                            break;
                                    }
                                }
                                else if(input.moveright){
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentItemUI = selectorUI.UI.Q("item" + (selectorUI.currentItem + 1).ToString());
                                    currentItemUI.AddToClassList("item");
                                    currentItemUI.RemoveFromClassList("item_selected");
                                    // change item
                                    selectorUI.currentItem++;
                                    if(selectorUI.currentItem >= 5){
                                        selectorUI.currentItem = 0;
                                    }
                                    VisualElement nextItemUI = selectorUI.UI.Q("item" + (selectorUI.currentItem + 1).ToString());
                                    nextItemUI.RemoveFromClassList("item");
                                    nextItemUI.AddToClassList("item_selected");
                                }
                                else if(input.moveleft){
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentItemUI = selectorUI.UI.Q("item" + (selectorUI.currentItem + 1).ToString());
                                    currentItemUI.AddToClassList("item");
                                    currentItemUI.RemoveFromClassList("item_selected");

                                    selectorUI.currentItem--;
                                    if(selectorUI.currentItem < 0){
                                        selectorUI.currentItem = 4;
                                    }
                                    VisualElement nextItemUI = selectorUI.UI.Q("item" + (selectorUI.currentItem + 1).ToString());
                                    nextItemUI.AddToClassList("item_selected");
                                    nextItemUI.RemoveFromClassList("item");
                                    //change item
                                }
                            }

                            break;
                        case menuType.attackMenu:
                            battleUI.visible = false;
                            itemDesc.visible = false;
                            enemySelector.visible = true;

                            if(input.moveleft){
                                AudioManager.playSound("menuchange");
                                currentEnemySelected--;
                            }
                            else if(input.moveright){
                                AudioManager.playSound("menuchange");
                                currentEnemySelected++;
                            }
                            if(currentEnemySelected == EnemyIds.Length){
                                currentEnemySelected = 0;
                            }
                            else if(currentEnemySelected < 0){
                                currentEnemySelected = EnemyIds.Length - 1;
                            }

                            foreach(Entity ent in enemyUiSelection){
                                    EnemySelectorData temp = GetComponent<EnemySelectorData>(ent);

                                    if(temp.isDead && temp.enemyId == EnemyIds[currentEnemySelected].id){
                                        currentEnemySelected++;
                                        if(currentEnemySelected == EnemyIds.Length){
                                            currentEnemySelected = 0;
                                        }
                                    }
                                    else if(temp.enemyId == EnemyIds[currentEnemySelected].id){
                                        temp.isSelected = true;
                                        EntityManager.SetComponentData(ent, temp);
                                    }
                                    else{
                                        temp.isSelected = false;
                                        EntityManager.SetComponentData(ent, temp);
                                    }

                                    
                            }

                            if(input.goback){
                                AudioManager.playSound("menuchange");
                                currentMenu = menuType.battleMenu;
                            }                           
                            if(input.goselected){
                                AudioManager.playSound("menuchange");
                                battleData.targetingId = EnemyIds[currentEnemySelected].id;
                                battleData.selected = selectables.attack;
                                battleData.damage = inventory.inventory[selectorUI.currentItem].weapon.power;
                                battleData.useTime = inventory.inventory[selectorUI.currentItem].useTime;
                                battleData.maxUseTime = inventory.inventory[selectorUI.currentItem].useTime;

                                inventory.inventory[selectorUI.currentItem].weapon.rechargeTime = inventory.inventory[selectorUI.currentItem].weapon.attackTime;                               

                                currentMenu = menuType.battleMenu;

                                selectorUI.isSelected = false;
                                selectorUI.isHovered = true;

                                battleUI.visible = true;
                                itemDesc.visible = true;
                                enemySelector.visible = false;

                                selectorUI.UI.RemoveFromClassList("selected");
                                selectorUI.UI.AddToClassList("hovering");

                                selectorUI.isSelectable = false;
                            }
                            break;
                    }
                    for(int i = 0; i <= 4; i++){
                            VisualElement currentItemUI = selectorUI.UI.Q("item" + (i + 1).ToString());
                            VisualElement itemFilter = currentItemUI.Q<VisualElement>("itemloader");
                            Item tempItem = inventory.inventory[i];
                            if(tempItem.itemType == ItemType.sword || tempItem.itemType == ItemType.axe || tempItem.itemType == ItemType.none){
                                if(tempItem.weapon.rechargeTime > 0){
                                    itemFilter.style.height =  100 * ((tempItem.weapon.attackTime - tempItem.weapon.rechargeTime)/tempItem.weapon.attackTime);
                                    inventory.inventory[i].weapon.rechargeTime = tempItem.weapon.rechargeTime - deltaTime;
                                }
                                else{
                                    itemFilter.style.height = 0f;
                                }
                            }
                    }
                    
                }
                else{
                    if(battleData.useTime > 0)
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");
                        useBar.style.width = 400 * ((battleData.maxUseTime - battleData.useTime) / battleData.maxUseTime);
                        battleData.useTime -= deltaTime;
                    }
                    else
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");

                        Debug.Log("use bar length" + useBar.style.width);
                        selectorUI.isSelectable = true;
                        AudioManager.playSound("menuavailable");
                        //play audio
                    }
                }
            }).Run();
        }
        enemyUiSelection.Dispose();
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        hasMoved = false;
    }
}

public enum menuType{
    battleMenu,
    attackMenu,
    skillMenu,
    itemsMenu
}
