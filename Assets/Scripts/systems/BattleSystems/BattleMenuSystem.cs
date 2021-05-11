using Unity.Entities;
using UnityEngine.UIElements;
using UnityEngine;
using Unity.Collections;
using System;

public class BattleMenuSystem : SystemBase
{
    selectables currentSelection = selectables.attack;
    VisualElement battleUI;
    VisualElement enemySelector;
    UIDocument UIDoc;

    InkDisplaySystem inkDisplaySystem;
    bool isPrintingVictoryData;
    public int test = 0;

    //private bool isBattleMenuOn = false;
    private menuType currentMenu;
    private int currentCharacterSelected;
    private int currentEnemySelected;
    public BattleSystem battleSystem;
    public MovementSystem movementSystem;

    public int playerNumber;
    public bool hasMoved;
    VisualTreeAsset enemySelectionUITemplate;

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    protected override void OnStartRunning(){
        currentMenu = menuType.battleMenu;

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        //getting the battle system then subscribing to the battle start event so it can activate the menu when the transition is done
        battleSystem = World.GetOrCreateSystem<BattleSystem>();
        battleSystem.OnBattleStart += WaitForTransition_OnBattleStart;
        battleSystem.OnBattleEnd += StartVictoryData_OnBattleEnd;

        movementSystem = World.GetExistingSystem<MovementSystem>();
        movementSystem.OnTransitionEnd += EnableMenu_OnTransitionEnd;

        enemySelectionUITemplate = Resources.Load<VisualTreeAsset>("EnemyDetails");
    }

    protected override void OnUpdate()
    {
            EntityManager.CompleteAllJobs();
            float deltaTime = Time.DeltaTime;

            EntityQuery enemyUiSelectionGroup = GetEntityQuery(typeof(EnemySelectorUI), typeof(EnemySelectorData));
            NativeArray<Entity> enemyUiSelection = enemyUiSelectionGroup.ToEntityArray(Allocator.TempJob);

            DynamicBuffer<EnemyBattleData> EnemyIds = new DynamicBuffer<EnemyBattleData>();

            EntityQuery battleCharacters = GetEntityQuery(typeof(CharacterStats), typeof(BattleData));

            EntityQuery uiInputQuery = GetEntityQuery(typeof(UIInputData));
            UIInputData input = uiInputQuery.GetSingleton<UIInputData>();

            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            Color black = Color.black;
            Color grey = Color.grey;

            /*EntityQuery caravanQuery = GetEntityQuery(typeof(CaravanTag));
            Entity caravan = caravanQuery.GetSingletonEntity();
            DynamicBuffer<WeaponData> weaponInventory = GetBuffer<WeaponData>(caravan);
            DynamicBuffer<ArmorData> armorInventory = GetBuffer<ArmorData>(caravan);
            DynamicBuffer<CharmData> charmInventory = GetBuffer<CharmData>(caravan);
            */
            Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((DynamicBuffer<ItemData> itemInventory, AnimationData animation, Animator animator, PlayerSelectorUI selectorUI,int entityInQueryIndex, ref BattleData battleData, ref CharacterStats characterStats, in Entity entity) =>{
                battleData.selected = selectables.none;

                Label healthText = selectorUI.UI.Q<Label>("health_text");
                VisualElement healthBarBase = selectorUI.UI.Q<VisualElement>("health_bar_base");
                VisualElement healthBar = selectorUI.UI.Q<VisualElement>("health_bar");

                healthBar.style.width = healthBarBase.contentRect.width * (characterStats.health / characterStats.maxHealth);
                healthText.text = "HP: " + characterStats.health.ToString() + "/" + characterStats.maxHealth.ToString();

                if(selectorUI.isSelectable){
                    VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");
                    useBar.style.width = 0;
                    switch (currentMenu){
                        case menuType.battleMenu:
                            battleUI.visible = true;
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
                                if(input.goback){
                                    AudioManager.playSound("menuchange");
                                    selectorUI.isSelected = false;
                                    selectorUI.isHovered = true;

                                    selectorUI.UI.RemoveFromClassList("selected");
                                    selectorUI.UI.AddToClassList("hovering");
                                }
                                if(input.goselected){
                                    switch(selectorUI.currentSelection){
                                        case battleSelectables.fight:
                                            currentMenu = menuType.attackMenu;
                                        break;
                                        case battleSelectables.skills:

                                        break;
                                        case battleSelectables.items:
                                        break;
                                    }
                                }
                                
                                if(input.moveright && !(selectorUI.currentSelection == battleSelectables.run)){
                                    Debug.Log(selectorUI.currentSelection.ToString());
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentItemUI = selectorUI.UI.Q(selectorUI.currentSelection.ToString());
                                    currentItemUI.AddToClassList("item");
                                    currentItemUI.RemoveFromClassList("item_selected");
                                    selectorUI.currentSelection++;

                                    VisualElement nextItemUI = selectorUI.UI.Q((selectorUI.currentSelection).ToString());
                                    nextItemUI.RemoveFromClassList("item");
                                    nextItemUI.AddToClassList("item_selected");
                                    
                                }
                                else if(input.moveleft && !(selectorUI.currentSelection == battleSelectables.fight)){
                                    AudioManager.playSound("menuchange");
                                    VisualElement currentItemUI = selectorUI.UI.Q(selectorUI.currentSelection.ToString());
                                    currentItemUI.AddToClassList("item");
                                    currentItemUI.RemoveFromClassList("item_selected");
                                    selectorUI.currentSelection--;

                                    VisualElement nextItemUI = selectorUI.UI.Q((selectorUI.currentSelection).ToString());
                                    nextItemUI.RemoveFromClassList("item");
                                    nextItemUI.AddToClassList("item_selected");
                                }
                            }

                            break;
                        case menuType.attackMenu:
                            battleUI.visible = false;
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
                                animator.Play(animation.basicSwordAnimationName);
                                AudioManager.playSound("swordswing");
                                battleData.targetingId = EnemyIds[currentEnemySelected].id;
                                battleData.selected = selectables.attack;
                                battleData.damage = characterStats.equipedWeapon.power;
                                battleData.useTime = characterStats.equipedWeapon.useTime;
                                battleData.maxUseTime = battleData.useTime;


                                //inventory.inventory[selectorUI.currentItem].weapon.rechargeTime = inventory.inventory[selectorUI.currentItem].weapon.attackTime;                               

                                currentMenu = menuType.battleMenu;

                                selectorUI.isSelected = false;
                                selectorUI.isHovered = true;

                                battleUI.visible = true;
                                enemySelector.visible = false;

                                selectorUI.UI.RemoveFromClassList("selected");
                                selectorUI.UI.AddToClassList("hovering");

                                selectorUI.isSelectable = false;
                            }
                            break;
                    }
                    /*
                    was used when weapons had their own recharge time
                    for(int i = 0; i <= 4; i++){
                            VisualElement currentItemUI = selectorUI.UI.Q("item" + (i + 1).ToString());
                            VisualElement itemFilter = currentItemUI.Q<VisualElement>("itemloader");
                            Item tempItem = inventory.inventory[i];
                            if(tempItem.itemType == ItemType.sword || tempItem.itemType == ItemType.axe || tempItem.itemType == ItemType.none){
                                if(tempItem.weapon.rechargeTime > 0){
                                    itemFilter.style.height =  currentItemUI.contentRect.height * ((tempItem.weapon.attackTime - tempItem.weapon.rechargeTime)/tempItem.weapon.attackTime);
                                    inventory.inventory[i].weapon.rechargeTime = tempItem.weapon.rechargeTime - deltaTime;
                                }
                                else{
                                    itemFilter.style.height = 0f;
                                }
                            }
                    }*/
                    
                }
                else{
                    if(battleData.useTime > 0)
                    {
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");

                        useBar.style.width = selectorUI.UI.contentRect.width * ((battleData.maxUseTime - battleData.useTime) / battleData.maxUseTime);
                        battleData.useTime -= deltaTime;
                    }
                    else
                    {
                        animator.Play(animation.swordIdleAnimationName);
                        VisualElement useBar = selectorUI.UI.Q<VisualElement>("useBar");

                        
                  
                        selectorUI.isSelectable = true;
                        AudioManager.playSound("menuavailable");
                        //play audio
                    }
                }
            }).Run();
            enemyUiSelection.Dispose();
        
        m_EndSimulationEcbSystem.AddJobHandleForProducer(this.Dependency);
        hasMoved = false;
    }
    private void DisableMenu_OnBattleEnd(System.Object sender, System.EventArgs e){
        battleUI.visible = false;
        enemySelector.visible = false;
    }
    private void EnableMenu_OnTransitionEnd(System.Object sender, System.EventArgs e){
        Debug.Log("menu's should be enabled");
        EntityQuery UIDocumentGroup = GetEntityQuery(typeof(UIDocument), typeof(BattleUITag));
        UIDoc = UIDocumentGroup.ToComponentArray<UIDocument>()[0];
        VisualElement root = UIDoc.rootVisualElement;
        battleUI = root.Q<VisualElement>("BattleUI");
        enemySelector = root.Q<VisualElement>("EnemySelector");
        battleUI.visible = true;

        EntityQuery characterQuery = GetEntityQuery(typeof(CharacterStats));
        NativeArray<CharacterStats> characters = characterQuery.ToComponentDataArray<CharacterStats>(Allocator.TempJob);
        NativeArray<Entity> characterEntities = characterQuery.ToEntityArray(Allocator.TempJob);
        // adding the selectorUI stuff to the players
        int i = 0;
        foreach(Entity entity in battleSystem.playerEntities){
            string tempstr = "character" + (i + 1).ToString();
            VisualElement currentCharacter = battleUI.Q<VisualElement>(tempstr);
            EntityManager.AddComponentObject(entity, new PlayerSelectorUI { UI = currentCharacter, currentSelection = battleSelectables.fight, isSelected = false, isHovered = i == 0 });
        }

        foreach(Entity entity in battleSystem.enemyEntities){   
            VisualElement newEnemySelectUI = enemySelectionUITemplate.CloneTree();
            CharacterStats characterStats = GetComponent<CharacterStats>(entity);
            EntityManager.AddComponentData(entity, new EnemySelectorData { enemyId = characterStats.id, isSelected = false });
            EntityManager.AddComponentObject(entity, new EnemySelectorUI{ enemySelectorUI = newEnemySelectUI});
        }
    }
    private void WaitForTransition_OnBattleStart(System.Object sender, System.EventArgs e){
        playerNumber = battleSystem.playerEntities.Count;
        Debug.Log("should now be subed");
        movementSystem.OnTransitionEnd += EnableMenu_OnTransitionEnd;
    }

    private void FinishVictoryData_OnWritingFinished(object sender, System.EventArgs e){
        inkDisplaySystem.OnWritingFinished -= FinishVictoryData_OnWritingFinished;
        isPrintingVictoryData = false;
        var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        // transition back once the writer is done
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((ref BeforeBattleData beforeBattleData, in Entity entity) =>
        {
            ecb.AddComponent(entity, new TransitionData{newPosition = beforeBattleData.previousLocation});
            ecb.RemoveComponent<BeforeBattleData>(entity);
        }).Run();
    }
    private void StartVictoryData_OnBattleEnd(object sender, OnBattleEndEventArgs e){
        AudioManager.stopSong("tempBattleMusic");
        if(e.isPlayerVictor){
            var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();

            EntityQuery battleCharacterGroup = GetEntityQuery(ComponentType.ReadWrite<CharacterStats>(), ComponentType.ReadWrite<BattleData>());
            NativeArray<Entity> battleCharacters = battleCharacterGroup.ToEntityArray(Allocator.TempJob);

            EntityQuery beforeBattleGroup = GetEntityQuery(typeof(BeforeBattleData));
            NativeArray<Entity> beforeBattleDatas = beforeBattleGroup.ToEntityArray(Allocator.TempJob);

            // if the player wins, give them some awards and give them some kind 
            // if the player loses, set them to their last respawn point

            //if a character loses or wins, say the results
            //since they aren't in a battle, remove the battle data
            foreach (Entity entity in battleCharacters){
                ecb.RemoveComponent<BattleData>(entity);
            }

            beforeBattleDatas.Dispose();
            battleCharacters.Dispose();

            isPrintingVictoryData = true;
            inkDisplaySystem.DisplayVictoryData();
            inkDisplaySystem.OnWritingFinished += FinishVictoryData_OnWritingFinished;
        }
        
    }
}

public enum menuType{
    battleMenu,
    attackMenu,
    skillMenu,
    itemsMenu
}
